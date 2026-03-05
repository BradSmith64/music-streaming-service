using music_streaming_application;
using music_streaming_domain.Songs;

public class UnlikeSongUseCase
{
    ISongRepository _repository;

    public UnlikeSongUseCase(ISongRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(UnlikeSongCommand command)
    {
        Song song = await _repository.GetSongByIdAsync(command.SongId);

        song.Unlike(command.UserId);

        await _repository.UnlikeSongAsync(song, command.UserId);
    }
}

public class UnlikeSongCommand
{
    public int SongId { get; set; }
    public int UserId { get; set; }
}