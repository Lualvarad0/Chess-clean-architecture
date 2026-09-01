using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoCaballo : IEspecificacionMovimientoPieza
{
    private static readonly (int DeltaFila, int DeltaColumna)[] Saltos =
    [
        (2, 1), (2, -1), (-2, 1), (-2, -1),
        (1, 2), (1, -2), (-1, 2), (-1, -2)
    ];

    public TipoPieza Tipo => TipoPieza.Caballo;

    public IEnumerable<Movimiento> MovimientosCandidatos(Tablero tablero, Posicion origen, Color color, ContextoMovimiento contexto)
    {
        foreach (var (deltaFila, deltaColumna) in Saltos)
        {
            if (!Posicion.TryDesplazar(origen, deltaFila, deltaColumna, out var destino))
                continue;

            var piezaDestino = tablero.ObtenerPiezaEn(destino);
            if (piezaDestino is null || piezaDestino.Value.Color != color)
                yield return new Movimiento(origen, destino);
        }
    }
}
