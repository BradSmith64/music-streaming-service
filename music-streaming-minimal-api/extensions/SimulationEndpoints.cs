using music_streaming_application;

namespace music_streaming_minimal_api.extensions;

public static class SimulationEndpoints
{
    public static void MapSimulationEndpoints(this IEndpointRouteBuilder app)
    {
#if DEBUG
        app.MapPost("/simulate-upload", async (string fileName, string uploaderId, RegisterSongUseCase useCase) =>
        {
            // Simulate what the Azure Function Broker will do:
            // 1. Construct the URI for the file in the landing zone
            // 2. Wrap it in a command
            
            // The Cloud Broker expects: landing-zone/{uploaderId}/{fileName}
            var blobUri = $"http://localhost:8080/landing-zone/{uploaderId}/{fileName}";

            var command = new RegisterSongCommand
            {
                BlobUri = blobUri,
                UploaderId = uploaderId
            };

            try
            {
                await useCase.ExecuteAsync(command);
                return Results.Ok(new { message = $"Simulation complete for user '{uploaderId}'. Check your 'music' folder and database." });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithSummary("SIMULATOR: Process a file from local landing zone")
        .WithDescription("Development only. Triggers the RegisterSongUseCase manually using a local file URI with UploaderId context.");
#endif
    }
}