# API Error Handling Strategy

This document tracks implemented and proposed improvements for standardized error handling in the `music-streaming-minimal-api`.

## 1. Implemented Improvements

### Structured Error Responses (`ErrorResponseDTO`)
- **Location**: `music-streaming-minimal-api\api\responses\ErrorResponse.cs`
- **Issue**: Errors were previously returned as anonymous objects (e.g., `new { error = "..." }`), making it difficult for clients to handle specific error cases programmatically and for Swagger to document the response structure.
- **Fix**: 
    - Created a formal `ErrorResponseDTO` with `ErrorMessage` and a machine-readable `ErrorCode`.
    - Updated `POST` and `DELETE` likes endpoints to return this DTO.
    - Added `.Produces<ErrorResponseDTO>` metadata to endpoints for Swagger documentation.

### Swashbuckle Integration (.NET 8.0)
- **Fix**: 
    - Integrated `Swashbuckle.AspNetCore` (Swagger) for API documentation.
    - Configured `AddEndpointsApiExplorer()` and `AddSwaggerGen()` in `Program.cs`.
    - Note: Switched from .NET 10 Native OpenAPI to Swashbuckle to ensure compatibility with Azure App Service Linux runtimes.

## 2. Backlog / To-Do

### Restore Response Descriptions in Swagger
- **Issue**: The custom `.WithResponseDescription()` extension method was simplified to a placeholder during the .NET 8.0/Swashbuckle migration. As a result, specific status code descriptions (e.g., "The song was successfully liked") are missing from the Swagger UI.
- **Goal**: Implement a custom `IOperationFilter` for Swashbuckle that can read custom metadata or attributes to restore these descriptions.

## 3. Proposed Improvements

### Standardized `ProblemDetails` (RFC 7807)
- **Target**: Global API error handling.
- **Proposal**: 
    - Transition from `ErrorResponseDTO` to the ASP.NET Core built-in `ProblemDetails` class.
    - Implement a global exception handler or middleware to automatically map domain exceptions (like `SongNotFoundException`) to standard `ProblemDetails` responses.
- **Benefit**: Adheres to the industry-standard RFC 7807.
