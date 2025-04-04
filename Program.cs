using AmadeusCitySearch.Amadeus;
using AmadeusCitySearch.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Register the AmadeusOptions class
builder.Services.Configure<AmadeusOptions>(builder.Configuration.GetSection(AmadeusOptions.Amadeus));  

// Register the AmadeusClient class
builder.Services.AddScoped<AmadeusClient>();

// Register all the HttpClient-related services
builder.Services.AddHttpClient();

// Register FluentValidation classes
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/search", async ([AsParameters] SearchRequestDto request, [FromServices] AmadeusClient amadeusClient,
    [FromServices] IValidator<SearchRequestDto> validator, CancellationToken cancellationToken) =>
{
    // First validate the incoming request and return an error if necessary
    var validationResult = await validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(
            validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray()));
    }

    try
    {
        // Make a response to the Amadeus API using the client
        var response = await amadeusClient.SearchCitiesAsync(request.Query, cancellationToken: cancellationToken);
        return Results.Ok(response.Data);
    }
    catch (AmadeusApiException e)
    {
        // Return any Amadeus Exception as is
        return Results.Problem(title: e.Title, statusCode: e.StatusCode, detail: e.Detail);
    }
    catch (AmadeusInvalidResponseException e)
    {
        // Return a 500 error if the response was invalid
        return Results.Problem(title: "Invalid response", statusCode: 500, detail: e.Message);
    }
})
.WithName("Search")
.WithOpenApi();

await app.RunAsync();