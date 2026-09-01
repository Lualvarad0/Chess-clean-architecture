using Ajedrez.Domain.Entidades;

namespace Ajedrez.Application.Interfaces;

public interface IPartidaRepository
{
    Task<Partida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task GuardarAsync(Partida partida, CancellationToken cancellationToken);
}
