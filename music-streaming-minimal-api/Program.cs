using Microsoft.EntityFrameworkCore;
using music_streaming_application;
using music_streaming_infrastructure;
using music_streaming_minimal_api;
using music_streaming_domain.Songs;
using Microsoft.Extensions.Options;
using music_streaming_minimal_api.extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? conn_string = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(conn_string))
{
    throw new InvalidOperationException("No connection string found. Please provide a DefaultConnection connection string.");
}

Console.WriteLine($"Using connection string: {conn_string}");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(conn_string);
});

builder.Services.AddScoped<GetSongsQueryHandler>();
builder.Services.AddScoped<LikeSongUseCase>();
builder.Services.AddScoped<UnlikeSongUseCase>();

builder.Services.AddScoped<ISongQueryService, SongQueryService_EntityFramework>();
builder.Services.AddScoped<ISongRepository, SongRepository_EntityFramework>();

// Configure storage based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISongStorage>(sp => new SongStorage_LocalHttpServer("http://localhost:8080/"));
}
else
{
    // Bind MusicStorage section to strongly typed class
    builder.Services.Configure<SongStorage_AzureBlobStorageOptions>(
        builder.Configuration.GetSection("SongStorage")
    );

    builder.Services.AddScoped<ISongStorage>((sp) =>
    {
        var options = sp.GetRequiredService<IOptions<SongStorage_AzureBlobStorageOptions>>().Value;
        return new SongStorage_AzureBlobStorage(options);
    });
}

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // <-- default Next.js port when running in dev
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.MapGet("/songs", async (GetSongsQueryHandler handler) =>
{
    var songs = await handler.Execute( new GetSongsQuery { UserId = 1 });
    
    var songsResponse = songs.Select( song =>
    {
       return new SongDTO
       {
            SongId = song.SongId,
            Title = song.Title,
            AlbumTitle = song.AlbumTitle,
            ArtistName = song.ArtistName,
            ReleaseDate = song.ReleaseDate,
            LikeCount = song.LikeCount,
            LikedByUser = song.LikedByUser,
            Url = song.Url
        };
    }).ToList();

    return new GetSongsResponseDTO { Items = songsResponse };
})
.WithSummary("Get all songs")
.WithDescription("Retrieves a list of all available songs, including whether the current user has liked them.");

app.MapPost("/songs/{id}/likes", async (int id, LikeSongUseCase likeSongUseCase) =>
{
    try
    {
        var likeId = await likeSongUseCase.Execute(new LikeSongCommand { SongId = id, UserId = 1 });
        return Results.Created($"/songs/{id}/likes/{likeId}", new LikeSongDTO { Id = likeId });
    }
    catch( SongNotFoundException snf )
    {
        Console.WriteLine(snf.Message);
        return Results.NotFound(new ErrorResponseDTO { ErrorMessage = $"The requested song (with ID { id }) was not found (LikeSongUseCase)", ErrorCode = "SONG_NOT_FOUND" });
    }
    catch( SongAlreadyLikedException sal )
    {
        Console.WriteLine(sal.Message);
        return Results.Conflict(new ErrorResponseDTO { ErrorMessage = $"The requested song (with ID { id }) has already been liked (LikeSongUseCase)", ErrorCode = "SONG_ALREADY_LIKED" });
    }
})
.Produces<LikeSongDTO>(StatusCodes.Status201Created)
.Produces<ErrorResponseDTO>(StatusCodes.Status404NotFound)
.Produces<ErrorResponseDTO>(StatusCodes.Status409Conflict)
.WithResponseDescription("201", "The song was successfully liked.")
.WithResponseDescription("404", "The requested song was not found.")
.WithResponseDescription("409", "The user has already liked this song.")
.WithSummary("Like a song")
.WithDescription("Allows a user to like a specific song by its ID.");

app.MapDelete("/songs/{id}/likes", async ( int id, UnlikeSongUseCase unlikeSongUseCase ) =>
{
    try
    {
        await unlikeSongUseCase.Execute(new UnlikeSongCommand { SongId = id, UserId = 1 });
        return Results.NoContent();
    }
    catch( SongNotFoundException snf )
    {
        Console.WriteLine(snf.Message);
        return Results.NotFound(new ErrorResponseDTO { ErrorMessage = $"The requested song (with ID { id }) was not found (UnlikeSongUseCase)", ErrorCode = "SONG_NOT_FOUND" });
    }
    catch( SongIsntLikedException sil )
    {
        Console.WriteLine(sil.Message);
        return Results.NotFound(new ErrorResponseDTO { ErrorMessage = $"The requested song (with ID { id }) isn't liked (UnlikeSongUseCase)", ErrorCode = "SONG_ISNT_LIKED" });
    }
})
.Produces(StatusCodes.Status204NoContent)
.Produces<ErrorResponseDTO>(StatusCodes.Status404NotFound)
.WithResponseDescription("204", "The song was successfully unliked.")
.WithResponseDescription("404", "The requested song was not found, or the user had not liked it yet.")
.WithSummary("Unlike a song")
.WithDescription("Allows a user to remove their like from a specific song.");

app.Run();
