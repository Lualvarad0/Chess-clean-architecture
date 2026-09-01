using Ajedrez.Application.Excepciones;
using Ajedrez.Application.Interfaces;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Queries.ObtenerEstadoPartida;

public sealed class ObtenerEstadoPartidaQueryHandler(IPartidaRepository repositorio)
    : IRequestHandler<ObtenerEstadoPartidaQuery, PartidaDto>
{
    public async Task<PartidaDto> Handle(ObtenerEstadoPartidaQuery request, CancellationToken cancellationToken)
    {
        var partida = await repositorio.ObtenerPorIdAsync(request.PartidaId, cancellationToken)
            ?? throw new PartidaNoEncontradaException(request.PartidaId);

        return MapeadorPartida.ADto(partida);
    }
}
