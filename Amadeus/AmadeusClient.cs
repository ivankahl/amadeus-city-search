using System.Net.Http.Headers;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace AmadeusCitySearch.Amadeus;  
  
public class AmadeusClient  
{  
    private readonly HttpClient _client;  
    private readonly AmadeusOptions _options;  
  
    private DateTimeOffset _nextRequestTime = DateTimeOffset.MinValue;  
  
    // Inject the AmadeusOptions and HttpClient
    public AmadeusClient(HttpClient client, IOptions<AmadeusOptions> options)  
    {  
        _client = client;  
        _options = options.Value;  
        
        // Set the base URL on the HttpClient
        _client.BaseAddress = new Uri(_options.BaseUrl);  
    }
    
    /// <summary>
    /// Searches for cities using the Amadeus City Search API.
    /// </summary>
    /// <param name="keyword">Keyword to use when searching for cities.</param>
    /// <param name="countryCode">Country code to restrict city search.</param>
    /// <param name="cancellationToken">Used to cancel the request early if necessary.</param>
    /// <returns>The response containing a list of cities from the API.</returns>
    /// <exception cref="AmadeusInvalidResponseException">Thrown if the response body could not be deserialized.</exception>
    /// <exception cref="AmadeusApiException">Thrown if the Amadeus API returns an exception.</exception>
    public async Task<SearchCitiesResponse> SearchCitiesAsync(string keyword, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        // First make sure you have a valid access token
        await RefreshTokenIfExpiredAsync(cancellationToken);

        // Construct query parameters to send to the Amadeus API
        var queryParameters = new QueryBuilder { { "keyword", keyword } };
        if (!string.IsNullOrEmpty(countryCode))
            queryParameters.Add("countryCode", countryCode);
        
        var response = await _client.GetAsync($"/v1/reference-data/locations/cities{queryParameters.ToQueryString()}", cancellationToken);

        // Check for any error messages in the response. If any occurred, throw the relevant
        // exception
        await EnsureSuccessfulResponseAsync(response, cancellationToken);

        // If all went well, parse the JSON response and return it
        return await response.Content.ReadFromJsonAsync<SearchCitiesResponse>(cancellationToken: cancellationToken) ??
               throw new AmadeusInvalidResponseException("Could not deserialize the response body");
    }
    
    /// <summary>
    /// Checks if the current access token has expired and retrieves a new one if it has.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to terminate execution early</param>
    private async Task RefreshTokenIfExpiredAsync(CancellationToken cancellationToken = default)
    {
        // Token is still valid so leave it
        if (DateTimeOffset.UtcNow < _nextRequestTime)
            return;

        // Make an OAuth 2.0 request to refresh the access token
        var result = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
        {
            Address = "/v1/security/oauth2/token",
            GrantType = "client_credentials",
            ClientId = _options.ApiKey,
            ClientSecret = _options.ApiSecret
        }, cancellationToken);

        // Recalculate the next request time and update the Bearer token
        _nextRequestTime = DateTimeOffset.UtcNow.AddSeconds(result.ExpiresIn);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
    }
    
    /// <summary>
    /// Method that checks if a request was successful. If it failed, it parses the error and throws it.
    /// </summary>
    /// <param name="response">The HTTP response to check</param>
    /// <param name="cancellationToken">Used to cancel the request early if necessary.</param>
    /// <exception cref="AmadeusInvalidResponseException">Thrown if the response could not be deserialized.</exception>
    /// <exception cref="AmadeusApiException">Thrown with error details returned from Amadeus API.</exception>
    private static async Task EnsureSuccessfulResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        // Don't throw exception if successful status code
        if (response.IsSuccessStatusCode)
            return;

        // If received a failed status code, deserialize the error response and throw an exception
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
        var error = errorResponse?.Errors.FirstOrDefault() ??
                    throw new AmadeusInvalidResponseException("Could not deserialize the error response body");

        throw new AmadeusApiException((int)response.StatusCode, error.Code, error.Title, error.Detail);
    }
}