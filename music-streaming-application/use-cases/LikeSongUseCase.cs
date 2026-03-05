using music_streaming_domain.Songs;

namespace music_streaming_application;

public class LikeSongUseCase
{
    private ISongRepository _repository;

    public LikeSongUseCase(ISongRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Execute(LikeSongCommand command)
    {
        Song song = await _repository.GetSongByIdAsync(command.SongId);

        var like = song.Like(command.UserId);

        int likeId = await _repository.LikeSongAsync(song, like);

        return likeId;
    }
}

public class LikeSongCommand
{
    public int SongId { get; set; }
    public int UserId { get; set; }
}