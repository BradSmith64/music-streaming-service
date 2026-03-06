using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace music_streaming_minimal_api.extensions;

public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Placeholder for response description for .NET 8 compatibility.
    /// In .NET 8 with Swashbuckle, this usually requires an IOperationFilter.
    /// For simplicity, we'll just return the builder.
    /// </summary>
    public static RouteHandlerBuilder WithResponseDescription(this RouteHandlerBuilder builder, string statusCode, string description)
    {
        return builder;
    }
}
