# Amadeus City Search API

Amadeus City Search API is a lightweight ASP.NET Core 8 web service that provides a simple interface to search for cities using the Amadeus API. This service wraps the Amadeus City Search API and handles authentication, error handling, and response formatting.

Key features:
- Search for cities by keyword
- Input validation using FluentValidation
- Proper error handling and response formatting
- OAuth 2.0 token management for Amadeus API
- Dockerized for easy deployment
- Swagger UI for API documentation and testing

## Prerequisites

To run this project, you'll need:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop) (optional, for containerized deployment)
- Amadeus API credentials (API Key and Secret)
  - Register at [Amadeus for Developers](https://developers.amadeus.com/) to obtain your credentials

## Installation Instructions

### Clone the repository

```bash
git clone https://github.com/ivankahl/amadeus-city-search.git
cd amadeus-city-search
```

### Restore dependencies

```bash
dotnet restore
```

## Running the API

### Using .NET CLI with User Secrets

1. Initialize user secrets for the project:

```bash
dotnet user-secrets init
```

2. Set your Amadeus API credentials:

```bash
dotnet user-secrets set "Amadeus:ApiKey" "<API_KEY>"
dotnet user-secrets set "Amadeus:ApiSecret" "<API_SECRET>"
```

3. Run the API:

```bash
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5202

### Using Docker with Environment Variables

1. Build the Docker image:

```bash
docker build -t amadeus-city-search .
```

2. Run the container with environment variables:

```bash
docker run -p 8080:8080 \
  -e "Amadeus__ApiKey=your-api-key" \
  -e "Amadeus__ApiSecret=your-api-secret" \
  -e "ASPNETCORE_ENVIRONMENT=Development" \
  --name amadeus-city-search \
  amadeus-city-search
```

Note: .NET uses double underscores (`__`) instead of colons (`:`) for configuration hierarchy. Include `ASPNETCORE_ENVIRONMENT=Development` to access Swagger.

The API will be available at http://localhost:8080.

## Testing the API

### Using Swagger UI

Once the application is running, navigate to `/swagger` in your browser:
- Local .NET: https://localhost:7042/swagger
- Docker: http://localhost:8080/swagger

### Using curl

Search for cities with a keyword:

```bash
# Basic search
curl -X GET "http://localhost:5202/search?query=new%20york" -H "accept: application/json"
```

### Response Example

```json
[
  {
    "type": "location",
    "subType": "city",
    "name": "NEW YORK",
    "iataCode": "NYC",
    "address": {
      "postalCode": "10001",
      "countryCode": "US",
      "stateCode": "NY"
    },
    "geoCode": {
      "latitude": 40.71417,
      "longitude": -74.00583
    }
  }
]
```

## Error Handling

The API handles various error scenarios and returns appropriate HTTP status codes with descriptive error messages:

- 400 Bad Request: Invalid input parameters
- 500 Internal Server Error: Unexpected errors or API failures
