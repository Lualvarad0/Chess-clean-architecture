namespace Ajedrez.Application.Excepciones;

public sealed class PartidaNoEncontradaException : Exception
{
    public Guid PartidaId { get; }

    public PartidaNoEncontradaException(Guid partidaId)
        : base($"No se encontró la partida '{partidaId}'.")
    {
        PartidaId = partidaId;
    }
}
