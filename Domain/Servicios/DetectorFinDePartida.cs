using System.Text;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

public sealed class DetectorFinDePartida : IDetectorFinDePartida
{
    private const int LimiteMediosMovimientosSinCaptura = 100; // regla de los 50 movimientos = 100 medios movimientos

    public ResultadoPartida? Evaluar(
        Tablero tablero,
        Color turno,
        ContextoMovimiento contexto,
        IValidadorMovimientos validador,
        IReadOnlyList<string> historialClavesPosicion,
        int contadorMediosMovimientosSinCaptura)
    {
        bool tieneMovimientosLegales = validador.TodosLosMovimientosLegales(tablero, turno, contexto).Any();

        if (!tieneMovimientosLegales)
        {
            bool enJaque = validador.ReyEnJaque(tablero, turno);
            return enJaque
                ? new ResultadoPartida(turno.Opuesto(), MotivoFinalizacion.JaqueMate)
                : new ResultadoPartida(null, MotivoFinalizacion.Ahogado);
        }

        if (contadorMediosMovimientosSinCaptura >= LimiteMediosMovimientosSinCaptura)
            return new ResultadoPartida(null, MotivoFinalizacion.Regla50Movimientos);

        var claveActual = ClavePosicion(tablero, turno, contexto);
        if (historialClavesPosicion.Count(clave => clave == claveActual) >= 3)
            return new ResultadoPartida(null, MotivoFinalizacion.RepeticionTriple);

        if (MaterialInsuficiente(tablero))
            return new ResultadoPartida(null, MotivoFinalizacion.MaterialInsuficiente);

        return null;
    }

    public string ClavePosicion(Tablero tablero, Color turno, ContextoMovimiento contexto)
    {
        var constructor = new StringBuilder();

        for (int fila = 0; fila < 8; fila++)
        {
            for (int columna = 0; columna < 8; columna++)
            {
                var pieza = tablero.ObtenerPiezaEn(Posicion.Crear(fila, columna));
                constructor.Append(pieza is { } p ? $"{(char)('a' + columna)}{fila}{p.Color}{p.Tipo}" : ".");
            }
        }

        constructor.Append('|').Append(turno);
        constructor.Append('|').Append(contexto.CasillaAlPaso);
        constructor.Append('|').Append(contexto.DerechosEnroque);

        return constructor.ToString();
    }

    private static bool MaterialInsuficiente(Tablero tablero)
    {
        var piezasMenores = Enum.GetValues<Color>()
            .SelectMany(color => tablero.PiezasDeColor(color))
            .Where(pc => pc.Pieza.Tipo != TipoPieza.Rey)
            .ToList();

        if (piezasMenores.Count == 0)
            return true; // rey contra rey

        if (piezasMenores.Count == 1 && piezasMenores[0].Pieza.Tipo is TipoPieza.Alfil or TipoPieza.Caballo)
            return true; // rey y una pieza menor contra rey solo

        if (piezasMenores.Count == 2 && piezasMenores.All(pc => pc.Pieza.Tipo == TipoPieza.Alfil))
        {
            bool mismoColorDeCasilla = ColorDeCasilla(piezasMenores[0].Posicion) == ColorDeCasilla(piezasMenores[1].Posicion);
            return mismoColorDeCasilla;
        }

        return false;
    }

    private static bool ColorDeCasilla(Posicion posicion) => (posicion.Fila + posicion.Columna) % 2 == 0;
}
