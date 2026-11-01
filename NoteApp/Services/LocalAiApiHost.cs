using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NoteApp;

public enum AiHostState
{
    AlreadyRunning,
    Started,
    BinaryNotFound,
    ProcessExited,
    NotResponding,
}

public sealed class LocalAiApiHost : IDisposable
{
    private static readonly HttpClient Probe = new()
    {
        Timeout = TimeSpan.FromSeconds(2),
    };

    private readonly string _baseUrl;
    private readonly string _healthUrl;
    private Process? _process;

    public LocalAiApiHost(string baseUrl)
    {
        _baseUrl = baseUrl;
        _healthUrl = new Uri(new Uri(baseUrl), "api/health").ToString();
    }

    public async Task<AiHostState> EnsureStartedAsync(
        CancellationToken cancellationToken = default)
    {
        if (await IsUpAsync(cancellationToken))
        {
            return AiHostState.AlreadyRunning;
        }

        var binary = FindApiBinary();
        if (binary is null)
        {
            return AiHostState.BinaryNotFound;
        }

        _process = StartProcess(binary);

        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_process.HasExited)
            {
                return AiHostState.ProcessExited;
            }

            if (await IsUpAsync(cancellationToken))
            {
                return AiHostState.Started;
            }

            await Task.Delay(400, cancellationToken);
        }

        return AiHostState.NotResponding;
    }

    public void Dispose()
    {
        if (_process is { HasExited: false })
        {
            try
            {
                _process.Kill(entireProcessTree: true);
                _process.WaitForExit(2000);
            }
            catch (InvalidOperationException)
            {
            }
        }

        _process?.Dispose();
        _process = null;
    }

    private async Task<bool> IsUpAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await Probe.GetAsync(_healthUrl, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }

    private Process StartProcess(string dllPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{dllPath}\" --urls {_baseUrl.TrimEnd('/')}",
            WorkingDirectory = Path.GetDirectoryName(dllPath)!,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException("Не удалось запустить процесс LocalAiApi.");
    }

    private static string? FindApiBinary()
    {
        var solutionRoot = FindSolutionRoot();
        if (solutionRoot is null)
        {
            return null;
        }

        foreach (var configuration in new[] { "Debug", "Release" })
        {
            var candidate = Path.Combine(
                solutionRoot,
                "LocalAiApi",
                "bin",
                configuration,
                "net10.0",
                "LocalAiApi.dll");

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static string? FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LocalJsonModule.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
