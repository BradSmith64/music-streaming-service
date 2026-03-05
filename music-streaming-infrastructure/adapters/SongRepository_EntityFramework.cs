using Microsoft.EntityFrameworkCore;
using music_streaming_application;
using music_streaming_domain.Songs;

namespace music_streaming_infrastructure;

public class SongRepository_EntityFramework : ISongRepository
{
    private AppDbContext _context;
    private ISongStorage _storage;

    public SongRepository_EntityFramework(AppDbContext context, ISongStorage storage)
    {
        _context = context;
        _storage = storage;
    }
    
    public async Task<int> LikeSongAsync(Song song, Like like)
    {
        var newLike = new Persistence.Like
        {
            SongId = song.Id,
            UserId = like.UserId,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _context.Likes.AddAsync(newLike);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (!await _context.Songs.AnyAsync(entity => entity.Id == song.Id))
            {
                throw new SongNotFoundException(song.Id);
            }
            throw;
        }

        return newLike.Id;
    }

    public async Task UnlikeSongAsync(Song song, int userId )
    {
        var entity = await _context.Likes.FirstOrDefaultAsync( l => l.SongId == song.Id && l.UserId == userId );

        if( entity == null )
        {
            if (!await _context.Songs.AnyAsync(s => s.Id == song.Id))
            {
                throw new SongNotFoundException(song.Id);
            }
            throw new SongIsntLikedException(song.Id);
        }

        _context.Likes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    async Task<Song> ISongRepository.GetSongByIdAsync(int songId)
    {
        Persistence.Song? entity = await _context.Songs
            .Include(song => song.Likes)
            .FirstOrDefaultAsync(song => song.Id == songId);

        if( entity == null )
        {
            throw new SongNotFoundException(songId);
        }

        return new Song
        {
            Id = entity.Id,
            Title = entity.Title,
            AlbumTitle = entity.AlbumTitle,
            Likes = entity.Likes.Select( like => new Like { Id = like.Id, SongId = like.SongId, UserId = like.UserId, CreatedAt = like.CreatedAt }).ToList(),
            FileName = entity.FileName
        };
    }
}