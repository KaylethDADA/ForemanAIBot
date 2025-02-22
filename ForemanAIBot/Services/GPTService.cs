using ForemanAIBot.DTOs;
using ForemanAIBot.Interfaces;
using ForemanAIBot.Options;
using ForemanAIBot.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForemanAIBot.Services;

public class GPTService : IAIService
{
    private readonly ApiClient _apiClient;
    private readonly AIConfiguration _config;
    private readonly ILogger<GPTService> _logger;
    
    public GPTService(ApiClient apiClient, IOptions<AIConfiguration> options, ILogger<GPTService> logger)
    {
        _apiClient = apiClient;
        _config = options.Value;
        _logger = logger;
    }

    public async Task<AIResponse> AskAIAsync(AIRequest request)
    {
        var roleKey = request.Role.ToString();
        var prompt = _config.Prompts?.GetValueOrDefault(roleKey);

        if (string.IsNullOrEmpty(prompt))
        {
            _logger.LogWarning("Промпт для роли '{Role}' не найден в конфигурации.", roleKey);
            throw new ArgumentException($"Промпт для роли '{roleKey}' не найден в конфигурации.");
        }

        var requestBody = new
        {
            model = _config.Model,
            messages = new[]
            {
                new { role = "system", content = prompt },
                new { role = "user", content = request.UserMessage }
            },
            max_tokens = _config.MaxTokens
        };

        try
        {
            var response = await _apiClient.PostAsync<object, GPTResponse>(
                _config.BaseUrl,
                requestBody,
                _config.ApiKey
            );

            if (response == null || string.IsNullOrEmpty(response.ResponseMessage))
            {
                _logger.LogError("Пустой ответ от GPT API.");
                return new AIResponse("Ошибка: пустой ответ от GPT.");
            }

            return new AIResponse(response.ResponseMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке запроса в GPT API.");
            return new AIResponse("Ошибка при обращении к GPT API.");
        }
    }
}