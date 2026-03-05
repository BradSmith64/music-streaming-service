using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;

namespace music_streaming_minimal_api.extensions;

public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a description to a specific HTTP response status code in the OpenAPI documentation.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/>.</param>
    /// <param name="statusCode">The HTTP status code (e.g., "200", "404").</param>
    /// <param name="description">The description to apply.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> for chaining.</returns>
    public static RouteHandlerBuilder WithResponseDescription(this RouteHandlerBuilder builder, string statusCode, string description)
    {
        return builder.AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            if (operation.Responses is not null && operation.Responses.TryGetValue(statusCode, out var response))
            {
                response.Description = description;
            }
            return Task.CompletedTask;
        });
    }
}
