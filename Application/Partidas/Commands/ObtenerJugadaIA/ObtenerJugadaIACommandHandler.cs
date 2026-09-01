using Ajedrez.Application.Excepciones;
using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Servicios;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.ObtenerJugadaIA;

public sealed class ObtenerJugadaIACommandHandler(
    IPartidaRepository repositorio,
    IMotorIA motorIA,
    IValidadorMovimientos validadorMovimientos,
    IDetectorFinDePartida detectorFin,
    IDespachadorEventosDominio despachador)
    : IRequestHandler<ObtenerJugadaIACommand, PartidaDto>
{
    public async Task<PartidaDto> Handle(ObtenerJugadaIACommand request, CancellationToken cancellationToken)
    {
        var partida = await repositorio.ObtenerPorIdAsync(request.PartidaId, cancellationToken)
            ?? throw new PartidaNoEncontradaException(request.PartidaId);

        var movimiento = await motorIA.ObtenerMejorMovimientoAsync(partida, request.Nivel, cancellationToken);

        partida.RealizarMovimiento(movimiento, validadorMovimientos, detectorFin);

        await repositorio.GuardarAsync(partida, cancellationToken);
        await despachador.DespacharYLimpiarAsync(partida, cancellationToken);

        return MapeadorPartida.ADto(partida);
    }
}
