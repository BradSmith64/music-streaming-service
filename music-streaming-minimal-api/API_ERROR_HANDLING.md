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

### Transition to Native OpenAPI (.NET 10+)
- **Fix**: 
    - Moved away from `Swashbuckle.AspNetCore` in favor of the built-in `Microsoft.AspNetCore.OpenApi` support (`builder.Services.AddOpenApi()`).
    - Integrated `Scalar.AspNetCore` for modern API documentation UI.
    - Implemented custom `WithResponseDescription` extension for clean response documentation.
    - This resolved deprecation warnings related to `.WithOpenApi()` and aligned the project with modern .NET standards.

## 2. Proposed Improvements

### Standardized `ProblemDetails` (RFC 7807)
- **Target**: Global API error handling.
- **Proposal**: 
    - Transition from `ErrorResponseDTO` to the ASP.NET Core built-in `ProblemDetails` class.
    - Use `builder.Services.AddProblemDetails()` in `Program.cs`.
    - Implement a global exception handler or middleware to automatically map domain exceptions (like `SongNotFoundException`) to standard `ProblemDetails` responses.
- **Benefit**: Adheres to the industry-standard RFC 7807, providing a consistent, extensible format for error details that is recognized by many client libraries and tools.
- **ErrorCode Mapping**: The custom `ErrorCode` can be included in the `Extensions` dictionary of `ProblemDetails`.
