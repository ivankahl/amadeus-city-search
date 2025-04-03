namespace AmadeusCitySearch.Amadeus;  
  
public class AmadeusOptions  
{  
    public const string Amadeus = "Amadeus";  
      
    public required string BaseUrl { get; set; }  
    public required string ApiKey { get; set; }  
    public required string ApiSecret { get; set; }  
}