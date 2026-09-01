using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoPeon : IEspecificacionMovimientoPieza
{
    public TipoPieza Tipo => TipoPieza.Peon;

    public IEnumerable<Movimiento> MovimientosCandidatos(Tablero tablero, Posicion origen, Color color, ContextoMovimiento contexto)
    {
        int direccion = color == Color.Blanco ? 1 : -1;
        int filaInicial = color == Color.Blanco ? 1 : 6;
        int filaPromocion = color == Color.Blanco ? 7 : 0;

        if (Posicion.TryDesplazar(origen, direccion, 0, out var unPaso) && tablero.EstaVacia(unPaso))
        {
            foreach (var movimiento in ConPromocionSiAplica(origen, unPaso, filaPromocion))
                yield return movimiento;

            if (origen.Fila == filaInicial
                && Posicion.TryDesplazar(origen, direccion * 2, 0, out var dosPasos)
                && tablero.EstaVacia(dosPasos))
            {
                yield return new Movimiento(origen, dosPasos);
            }
        }

        foreach (int deltaColumna in new[] { -1, 1 })
        {
            if (!Posicion.TryDesplazar(origen, direccion, deltaColumna, out var destino))
                continue;

            var piezaDestino = tablero.ObtenerPiezaEn(destino);
            if (piezaDestino is { } capturada && capturada.Color != color)
            {
                foreach (var movimiento in ConPromocionSiAplica(origen, destino, filaPromocion))
                    yield return movimiento;
            }
            else if (piezaDestino is null && destino == contexto.CasillaAlPaso)
            {
                yield return new Movimiento(origen, destino, EsCapturaAlPaso: true);
            }
        }
    }

    private static IEnumerable<Movimiento> ConPromocionSiAplica(Posicion origen, Posicion destino, int filaPromocion)
    {
        if (destino.Fila != filaPromocion)
        {
            yield return new Movimiento(origen, destino);
            yield break;
        }

        yield return new Movimiento(origen, destino, PromocionA: TipoPieza.Dama);
        yield return new Movimiento(origen, destino, PromocionA: TipoPieza.Torre);
        yield return new Movimiento(origen, destino, PromocionA: TipoPieza.Alfil);
        yield return new Movimiento(origen, destino, PromocionA: TipoPieza.Caballo);
    }
}
