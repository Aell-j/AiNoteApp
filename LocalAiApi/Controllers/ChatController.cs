using LocalAiApi.Models;
using LocalAiApi.Services;

using Microsoft.AspNetCore.Mvc;
//Папка Controllers -> Файл : ChatController.cs
namespace LocalAiApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class ChatController(PromptProvider promptProvider 
        , OllamaService ollamaService) : ControllerBase
    {
        //health 
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }
        //prompts 
        [HttpGet("prompts")]
        public IActionResult GetPrompts()
        {
            return Ok(promptProvider.GetKeys());
        }

        //Можно как вариант сделать API для раздела PROMPTS, которая будет дополнять
        //Ключи и значения в Dictionary (что-то типо пост запросов)

        //chat
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody]ChatRequest request , CancellationToken cancellationToken)
        {
            if (request.Message == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Сообщение для Чата , не может быть пустым");
            }
            
            var systemPrompt = promptProvider.GetValue(request.Prompt);

            if (systemPrompt == "Промпт не найден")
            {
                return BadRequest("Указанный промпт не найден");
            }
            try
            {
                var userMSG = await ollamaService.ChatAsync
                    (
                    request.Message,
                    systemPrompt,
                    cancellationToken
                    );

                return Ok(userMSG);
            } catch (HttpRequestException)
            {
                return StatusCode(502, "ИИ недоступна(Ollama не ответила на запрос)");
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, "ИИ Вернула неккоректный ответ");
            }
        }

    }
}
