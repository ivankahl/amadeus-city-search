namespace AmadeusCitySearch.Amadeus;

/**
 * Models common to multiple requests
 */
public record MetaLinks(string Self);
public record Meta(long Count, MetaLinks Links);

public record IncludedAirport(string Name, string IataCode, string SubType);
public record Included(Dictionary<string, IncludedAirport> Airports);

public record GeoCode(decimal Latitude, decimal Longitude);
public record Address(string? PostalCode, string CountryCode, string? StateCode);

/**
 * Error Response
 */
public record Error(int StatusCode, int Code, string Title, string? Detail);
public record ErrorResponse(Error[] Errors);

/**
 * Search Cities Request/Response
 */
public record SearchCitiesResponseData(
    string Type,
    string SubType,
    string Name,
    string IataCode,
    Address Address,
    GeoCode GeoCode);

public record SearchCitiesResponse(SearchCitiesResponseData[] Data, Meta Meta, Included Included);