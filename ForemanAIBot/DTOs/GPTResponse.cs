namespace ForemanAIBot.DTOs;

public sealed record GPTResponse
{ 
    public string ResponseMessage { get; init; } = string.Empty;
}