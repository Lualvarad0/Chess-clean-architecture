using Ajedrez.Application.Excepciones;
using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Queries.ObtenerMovimientosLegales;

public sealed class ObtenerMovimientosLegalesQueryHandler(IPartidaRepository repositorio, IValidadorMovimientos validadorMovimientos)
    : IRequestHandler<ObtenerMovimientosLegalesQuery, MovimientosLegalesDto>
{
    public async Task<MovimientosLegalesDto> Handle(ObtenerMovimientosLegalesQuery request, CancellationToken cancellationToken)
    {
        var partida = await repositorio.ObtenerPorIdAsync(request.PartidaId, cancellationToken)
            ?? throw new PartidaNoEncontradaException(request.PartidaId);

        var origen = Posicion.DesdeNotacionAlgebraica(request.Casilla);
        var contexto = new ContextoMovimiento(partida.CasillaAlPaso, partida.DerechosEnroque);

        var destinos = validadorMovimientos
            .MovimientosLegales(partida.Tablero, origen, partida.TurnoActual, contexto)
            .Select(m => m.Destino.ANotacionAlgebraica())
            .Distinct()
            .ToList();

        return new MovimientosLegalesDto(destinos);
    }
}
