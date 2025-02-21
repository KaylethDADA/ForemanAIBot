using AiAPIStub.Dtos;
using AiAPIStub.Models;
using Microsoft.AspNetCore.Mvc;

namespace AiAPIStub.Controllers;

[ApiController]
[Route("v1/chat/completions")] 
public class ChatController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateChatCompletion([FromBody] ChatCompletionRequest request)
    {
        if (request.Messages == null || !request.Messages.Any())
        {
            return BadRequest("Сообщения отсутствуют.");
        }

        var userMessage = request.Messages.LastOrDefault(m => m.Role == "user")?.Content;

        var responseContent = GenerateResponse(userMessage);

        var response = new ChatCompletionResponse
        {
            Id = "chatcmpl-abc123",
            Object = "chat.completion",
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Model = request.Model,
            Usage = new Usage
            {
                PromptTokens = request.Messages.Sum(m => m.Content.Length) / 4,
                CompletionTokens = responseContent.Length / 4,
                TotalTokens = (request.Messages.Sum(m => m.Content.Length) / 4) + (responseContent.Length / 4)
            },
            Choices = new List<Choice>
            {
                new Choice
                {
                    Message = new Message
                    {
                        Role = "assistant",
                        Content = responseContent
                    },
                    Logprobs = null,
                    FinishReason = "stop",
                    Index = 0
                }
            }
        };

        return Ok(response);
    }
    private string GenerateResponse(string userPrompt)
    {
        if (string.IsNullOrEmpty(userPrompt))
        {
            return "Пожалуйста, задайте вопрос.";
        }

        var responses = new Dictionary<string, string>
        {
            // Вопросы и ответы для маляра
            { "Как покрасить стену?", "Используйте валик и краску. Нанесите краску равномерно." },
            { "Какая краска лучше?", "Лучше использовать водоэмульсионную краску." },
            { "Как подготовить стену к покраске?", "Очистите стену от пыли и нанесите грунтовку." },

            // Вопросы и ответы для гипсокартонщика
            { "Как установить гипсокартон?", "Закрепите гипсокартон на каркасе с помощью саморезов." },
            { "Как выровнять стену?", "Используйте шпаклевку и шлифовальную сетку." },
            { "Как сделать перегородку из гипсокартона?", "Соберите каркас из металлических профилей и обшейте гипсокартоном." },

            // Вопросы и ответы для электрика
            { "Как подключить розетку?", "Отключите электричество и следуйте инструкции." },
            { "Как заменить проводку?", "Обратитесь к профессиональному электрику." },
            { "Как установить светильник?", "Закрепите светильник на потолке и подключите провода." }
        };

        // Ищем ответ по вопросу пользователя
        if (responses.TryGetValue(userPrompt, out var response))
        {
            return response;
        }

        return "Извините, я не могу ответить на этот вопрос.";
    }
}