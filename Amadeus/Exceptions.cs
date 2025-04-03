namespace AmadeusCitySearch.Amadeus;  
  
public class AmadeusInvalidResponseException(string message) : Exception(message);  
  
public class AmadeusApiException(int statusCode, int code, string title, string? detail)  
    : Exception($"{title}: {detail} ({code})")  
{  
    public readonly int StatusCode = statusCode;  
    public readonly int Code = code;  
    public readonly string Title = title;  
    public readonly string? Detail = detail;  
}