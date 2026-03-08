using Microsoft.EntityFrameworkCore;
using music_streaming_application;
using music_streaming_domain.Songs;

namespace music_streaming_infrastructure;

public class SongRepository_EntityFramework : ISongRepository
{
    private AppDbContext _context;

    public SongRepository_EntityFramework(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Song> GetSongByIdAsync(int songId)
    {
        var entity = await _context.Songs
            .Include(s => s.Album)
                .ThenInclude(a => a.Artist)
            .Include(s => s.Likes)
            .FirstOrDefaultAsync(s => s.Id == songId);

        if (entity == null)
        {
            throw new SongNotFoundException(songId);
        }

        return MapToDomain(entity);
    }

    public async Task<Song?> GetSongByTitleAndAlbumAsync(string title, string albumTitle)
    {
        var entity = await _context.Songs
            .Include(s => s.Album)
                .ThenInclude(a => a.Artist)
            .Include(s => s.Likes)
            .FirstOrDefaultAsync(s => s.Title == title && s.Album.Title == albumTitle);

        return entity != null ? MapToDomain(entity) : null;
    }

    public async Task<int> AddSongAsync(Song song)
    {
        var entity = new Persistence.Song
        {
            Title = song.Title,
            AlbumId = song.Album.Id, // Extract from domain object
            Album = null!,
            ReleaseDate = song.ReleaseDate,
            FileName = song.FileName,
            Likes = new List<Persistence.Like>()
        };

        await _context.Songs.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Artist?> GetArtistByNameAsync(string name)
    {
        var entity = await _context.Artists.FirstOrDefaultAsync(a => a.Name == name);
        if (entity == null) return null;

        return new Artist { Id = entity.Id, Name = entity.Name };
    }

    public async Task<int> AddArtistAsync(Artist artist)
    {
        var entity = new Persistence.Artist { Name = artist.Name };
        await _context.Artists.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Album?> GetAlbumByTitleAndArtistAsync(string title, int artistId)
    {
        var entity = await _context.Albums
            .Include(a => a.Artist)
            .FirstOrDefaultAsync(a => a.Title == title && a.ArtistId == artistId);

        if (entity == null) return null;

        return new Album
        {
            Id = entity.Id,
            Title = entity.Title,
            Artist = new Artist { Id = entity.Artist.Id, Name = entity.Artist.Name }
        };
    }

    public async Task<int> AddAlbumAsync(Album album)
    {
        var entity = new Persistence.Album
        {
            Title = album.Title,
            ArtistId = album.Artist.Id, // Extract from domain object
            Artist = null!
        };
        await _context.Albums.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<int> LikeSongAsync(Song song, Like like)
    {
        var newLike = new Persistence.Like
        {
            SongId = song.Id,
            UserId = like.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Likes.AddAsync(newLike);
        await _context.SaveChangesAsync();
        return newLike.Id;
    }

    public async Task UnlikeSongAsync(Song song, int userId)
    {
        var entity = await _context.Likes.FirstOrDefaultAsync(l => l.SongId == song.Id && l.UserId == userId);
        if (entity != null)
        {
            _context.Likes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private Song MapToDomain(Persistence.Song entity)
    {
        return new Song
        {
            Id = entity.Id,
            Title = entity.Title,
            Album = new Album
            {
                Id = entity.Album.Id,
                Title = entity.Album.Title,
                Artist = new Artist { Id = entity.Album.Artist.Id, Name = entity.Album.Artist.Name }
            },
            ReleaseDate = entity.ReleaseDate,
            FileName = entity.FileName,
            Likes = entity.Likes.Select(l => new Like
            {
                Id = l.Id,
                SongId = l.SongId,
                UserId = l.UserId,
                CreatedAt = l.CreatedAt
            }).ToList()
        };
    }
}