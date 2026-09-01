using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

public interface IDetectorFinDePartida
{
    ResultadoPartida? Evaluar(
        Tablero tablero,
        Color turno,
        ContextoMovimiento contexto,
        IValidadorMovimientos validador,
        IReadOnlyList<string> historialClavesPosicion,
        int contadorMediosMovimientosSinCaptura);

    string ClavePosicion(Tablero tablero, Color turno, ContextoMovimiento contexto);
}
