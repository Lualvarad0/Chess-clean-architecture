using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Shared.Dtos;

namespace Ajedrez.Application.Partidas;

internal static class MapeadorPartida
{
    public static PartidaDto ADto(Partida partida)
    {
        var piezas = new List<CasillaDto>();

        foreach (var color in new[] { Color.Blanco, Color.Negro })
        {
            foreach (var (posicion, pieza) in partida.Tablero.PiezasDeColor(color))
                piezas.Add(new CasillaDto(posicion.ANotacionAlgebraica(), pieza.Tipo.ToString(), pieza.Color.ToString()));
        }

        var historial = HistorialADto(partida.Historial);

        var resultado = partida.Resultado is { } r
            ? new ResultadoPartidaDto(r.Ganador?.ToString(), r.Motivo.ToString())
            : null;

        return new PartidaDto(
            partida.Id,
            new TableroDto(piezas),
            partida.TurnoActual.ToString(),
            partida.Estado.ToString(),
            historial,
            resultado);
    }

    // El historial de dominio (Movimiento) no guarda qué pieza se movió — solo origen/destino,
    // eso se deduce del tablero en el momento de jugarlo. Para el DTO se reproduce la partida
    // desde el inicio y se consulta el tablero justo antes de aplicar cada jugada.
    private static List<MovimientoDto> HistorialADto(IReadOnlyList<Movimiento> historial)
    {
        var resultado = new List<MovimientoDto>();
        var tableroReplay = Tablero.PosicionInicial();

        foreach (var movimiento in historial)
        {
            var piezaMovida = tableroReplay.ObtenerPiezaEn(movimiento.Origen)!.Value.Tipo;

            resultado.Add(new MovimientoDto(
                movimiento.Origen.ANotacionAlgebraica(),
                movimiento.Destino.ANotacionAlgebraica(),
                piezaMovida.ToString(),
                movimiento.PromocionA?.ToString()));

            tableroReplay = tableroReplay.AplicarMovimiento(movimiento);
        }

        return resultado;
    }
}
