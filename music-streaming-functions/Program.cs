using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using music_streaming_application;
using music_streaming_infrastructure;
using music_streaming_domain.Songs;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        string? connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = context.Configuration["ConnectionStrings:DefaultConnection"];
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // Register Application Use Cases
        services.AddScoped<RegisterSongUseCase>();

        // Register Infrastructure Adapters
        services.AddScoped<IMetadataService, MetadataService_TagLib>();
        services.AddScoped<ISongRepository, SongRepository_EntityFramework>();
        
        // Use Azure Blob Storage for functions
        services.Configure<SongStorage_AzureBlobStorageOptions>(options => 
        {
            options.AccountName = context.Configuration["SongStorage:AccountName"] ?? "";
            options.AccountKey = context.Configuration["SongStorage:AccountKey"] ?? "";
            options.ContainerName = context.Configuration["SongStorage:ContainerName"] ?? "";
            options.ExpiryMinutes = int.TryParse(context.Configuration["SongStorage:ExpiryMinutes"], out var expiry) ? expiry : 60;
        });

        services.AddScoped<ISongStorage, SongStorage_AzureBlobStorage>();
    })
    .Build();

host.Run();