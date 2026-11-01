using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public class AiAnalysisService : IAiAnalysisService
{
    private const string DefaultPrompt = "analyse";

    private readonly HttpClient _http;

    public AiAnalysisService(string baseUrl)
        : this(new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMinutes(5),
        })
    {
    }

    public AiAnalysisService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _http.GetAsync("api/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }

    public async Task<string> AnalyseNoteAsync(
        Note note,
        string? folderTitle,
        CancellationToken cancellationToken = default)
    {
        var body = new JsonObject
        {
            ["Message"] = BuildNotePayload(note, folderTitle),
            ["Prompt"] = DefaultPrompt,
        }.ToJsonString();

        try
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/json");
            using var response = await _http.PostAsync("api/chat", content, cancellationToken);
            var text = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(FailureMessage(response.StatusCode, text));
            }

            // Контроллер отдаёт текст анализа как есть, без JSON-обёртки.
            return string.IsNullOrWhiteSpace(text)
                ? "Модель вернула пустой ответ. Попробуй ещё раз."
                : text.Trim();
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "ИИ не ответила вовремя: запрос длился больше 5 минут.");
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException(
                "Нет связи с LocalAiApi. Проверь, что API запущено и Ollama отвечает.");
        }
    }

    /// <summary>
    /// Заметка передаётся моделью одним JSON-объектом, чтобы промпт всегда собирался одинаково.
    /// </summary>
    private static string BuildNotePayload(Note note, string? folderTitle) => new JsonObject
    {
        ["NoteTitle"] = note.Title,
        ["NoteContent"] = note.Content,
        ["NoteFolder"] = folderTitle ?? "Без папки",
        ["NoteType"] = note.NoteType ?? "не задан",
        ["IsFavorite"] = note.IsFavorite,
        ["UpdatedAt"] = note.UpdatedAt.ToString("yyyy-MM-dd HH:mm"),
    }.ToJsonString();

    private static string FailureMessage(HttpStatusCode statusCode, string body)
    {
        var detail = Shorten(body);

        return statusCode switch
        {
            HttpStatusCode.BadRequest when detail.Contains("промпт") =>
                "API не знает такой промпт: " + detail,
            HttpStatusCode.BadRequest =>
                "API отклонило запрос: " + detail,
            HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout =>
                "ИИ недоступна: " + (detail.Length > 0 ? detail : "Ollama не ответила."),
            _ => "API ответило ошибкой " + (int)statusCode + ": " + detail,
        };
    }

    private static string Shorten(string text)
    {
        var flat = (text ?? string.Empty).Trim();
        return flat.Length <= 200 ? flat : flat[..200] + "…";
    }
}
