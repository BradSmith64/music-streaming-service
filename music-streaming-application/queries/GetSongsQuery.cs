using music_streaming_domain;

namespace music_streaming_application;

public class GetSongsQueryHandler
{
    private ISongQueryService _songQueryService;

    public GetSongsQueryHandler( ISongQueryService songQueryService )
    {
        _songQueryService = songQueryService;
    }

    public async Task<List<SongMetadata>> Execute(GetSongsQuery query)
    {
        var songs = await _songQueryService.GetSongsAsync(query.UserId);

        return songs;
    }
}

public class GetSongsQuery
{
    public int UserId { get; set; }
}