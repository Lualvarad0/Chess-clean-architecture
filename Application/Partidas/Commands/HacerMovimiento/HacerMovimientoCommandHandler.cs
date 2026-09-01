using Ajedrez.Application.Excepciones;
using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.HacerMovimiento;

public sealed class HacerMovimientoCommandHandler(
    IPartidaRepository repositorio,
    IValidadorMovimientos validadorMovimientos,
    IDetectorFinDePartida detectorFin,
    IDespachadorEventosDominio despachador)
    : IRequestHandler<HacerMovimientoCommand, PartidaDto>
{
    public async Task<PartidaDto> Handle(HacerMovimientoCommand request, CancellationToken cancellationToken)
    {
        var partida = await repositorio.ObtenerPorIdAsync(request.PartidaId, cancellationToken)
            ?? throw new PartidaNoEncontradaException(request.PartidaId);

        var movimiento = new Movimiento(
            Posicion.DesdeNotacionAlgebraica(request.Origen),
            Posicion.DesdeNotacionAlgebraica(request.Destino),
            request.PromocionA is null ? null : Enum.Parse<TipoPieza>(request.PromocionA, ignoreCase: true));

        partida.RealizarMovimiento(movimiento, validadorMovimientos, detectorFin);

        await repositorio.GuardarAsync(partida, cancellationToken);
        await despachador.DespacharYLimpiarAsync(partida, cancellationToken);

        return MapeadorPartida.ADto(partida);
    }
}
