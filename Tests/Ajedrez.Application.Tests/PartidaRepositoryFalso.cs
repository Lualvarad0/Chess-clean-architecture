using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Entidades;

namespace Ajedrez.Application.Tests;

public sealed class PartidaRepositoryFalso : IPartidaRepository
{
    private readonly Dictionary<Guid, Partida> _partidas = [];

    public Task<Partida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_partidas.GetValueOrDefault(id));

    public Task GuardarAsync(Partida partida, CancellationToken cancellationToken)
    {
        _partidas[partida.Id] = partida;
        return Task.CompletedTask;
    }
}
