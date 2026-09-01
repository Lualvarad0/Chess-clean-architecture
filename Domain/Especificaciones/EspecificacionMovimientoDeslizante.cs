using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Especificaciones;

public abstract class EspecificacionMovimientoDeslizante : IEspecificacionMovimientoPieza
{
    public abstract TipoPieza Tipo { get; }

    protected abstract (int DeltaFila, int DeltaColumna)[] Direcciones { get; }

    public IEnumerable<Movimiento> MovimientosCandidatos(Tablero tablero, Posicion origen, Color color, ContextoMovimiento contexto)
    {
        foreach (var (deltaFila, deltaColumna) in Direcciones)
        {
            var actual = origen;

            while (Posicion.TryDesplazar(actual, deltaFila, deltaColumna, out var destino))
            {
                var piezaDestino = tablero.ObtenerPiezaEn(destino);

                if (piezaDestino is null)
                {
                    yield return new Movimiento(origen, destino);
                    actual = destino;
                    continue;
                }

                if (piezaDestino.Value.Color != color)
                    yield return new Movimiento(origen, destino);

                break;
            }
        }
    }
}
