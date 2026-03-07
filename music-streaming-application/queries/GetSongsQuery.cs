using music_streaming_domain;

namespace music_streaming_application;

public class GetSongsQueryHandler
{
    private ISongQueryService _songQueryService;
    private ISongStorage _storage;

    public GetSongsQueryHandler( ISongQueryService songQueryService, ISongStorage storage )
    {
        _songQueryService = songQueryService;
        _storage = storage;
    }

    public async Task<List<SongMetadata>> Execute(GetSongsQuery query)
    {
        var songs = await _songQueryService.GetSongsAsync(query.UserId);

        foreach( var song in songs )
        {
            if( ! string.IsNullOrEmpty(song.FileName) )
            {
                song.Url = _storage.GetFileUri(song.FileName);
            }
        }

        return songs;
    }
}

public class GetSongsQuery
{
    public int UserId { get; set; }
}