using System.Text.Json.Serialization;

namespace ForemanAIBot.DTOs;

public sealed record GPTResponse
{
    [JsonPropertyName("choices")]
    public List<GPTChoice>? Choices { get; init; }

    public string ResponseMessage => Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
}

public sealed record GPTChoice
{
    [JsonPropertyName("message")]
    public GPTMessage? Message { get; init; }
}

public sealed record GPTMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}