using Microsoft.EntityFrameworkCore;
using music_streaming_application;
using music_streaming_domain;
using music_streaming_infrastructure;

public class SongQueryService_EntityFramework : ISongQueryService
{
    private AppDbContext _context;

    public SongQueryService_EntityFramework(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SongMetadata>> GetSongsAsync(int userId)
    {
        var songs = await _context.Songs
            .AsNoTracking()
            .Select( song => new
            {
                SongId = song.Id,
                Title = song.Title,
                AlbumTitle = song.AlbumTitle,
                ReleaseDate = song.ReleaseDate,
                LikeCount = song.Likes.Count,
                LikedByUser = song.Likes.Any( like => like.UserId == userId ),
                FileName = song.FileName
            })
            .ToListAsync();

        var songMetadata = songs.Select( song => new SongMetadata
        {
            SongId = song.SongId,
            Title = song.Title,
            AlbumTitle = song.AlbumTitle,
            ReleaseDate = song.ReleaseDate,
            LikeCount = song.LikeCount,
            LikedByUser = song.LikedByUser,
            FileName = song.FileName,
            Url = null // URL is enriched in the application layer
        }).ToList();

        return songMetadata;
    }
}