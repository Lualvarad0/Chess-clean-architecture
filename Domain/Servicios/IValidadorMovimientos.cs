using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

public interface IValidadorMovimientos
{
    bool ReyEnJaque(Tablero tablero, Color color);

    IEnumerable<Movimiento> MovimientosLegales(Tablero tablero, Posicion origen, Color turno, ContextoMovimiento contexto);

    IEnumerable<Movimiento> TodosLosMovimientosLegales(Tablero tablero, Color turno, ContextoMovimiento contexto);
}
