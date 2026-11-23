namespace HotelManagement.Api.Dtos.Common;

public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}