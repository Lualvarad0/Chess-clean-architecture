using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoRey : IEspecificacionMovimientoPieza
{
    private static readonly (int DeltaFila, int DeltaColumna)[] Direcciones =
    [
        (1, 0), (-1, 0), (0, 1), (0, -1),
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    ];

    public TipoPieza Tipo => TipoPieza.Rey;

    public IEnumerable<Movimiento> MovimientosCandidatos(Tablero tablero, Posicion origen, Color color, ContextoMovimiento contexto)
    {
        foreach (var (deltaFila, deltaColumna) in Direcciones)
        {
            if (!Posicion.TryDesplazar(origen, deltaFila, deltaColumna, out var destino))
                continue;

            var piezaDestino = tablero.ObtenerPiezaEn(destino);
            if (piezaDestino is null || piezaDestino.Value.Color != color)
                yield return new Movimiento(origen, destino);
        }

        foreach (var movimiento in CandidatosEnroque(tablero, origen, color, contexto.DerechosEnroque))
            yield return movimiento;
    }

    // Solo valida casillas vacías entre rey y torre; la legalidad de "no pasar por jaque"
    // la resuelve ValidadorMovimientos, que sí tiene visibilidad del resto del tablero.
    private static IEnumerable<Movimiento> CandidatosEnroque(Tablero tablero, Posicion origen, Color color, DerechosEnroque derechos)
    {
        int fila = color == Color.Blanco ? 0 : 7;

        if (derechos.TieneEnroqueCorto(color)
            && tablero.EstaVacia(Posicion.Crear(fila, 5))
            && tablero.EstaVacia(Posicion.Crear(fila, 6)))
        {
            yield return new Movimiento(origen, Posicion.Crear(fila, 6), EsEnroqueCorto: true);
        }

        if (derechos.TieneEnroqueLargo(color)
            && tablero.EstaVacia(Posicion.Crear(fila, 1))
            && tablero.EstaVacia(Posicion.Crear(fila, 2))
            && tablero.EstaVacia(Posicion.Crear(fila, 3)))
        {
            yield return new Movimiento(origen, Posicion.Crear(fila, 2), EsEnroqueLargo: true);
        }
    }
}
