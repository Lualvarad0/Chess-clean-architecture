using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Especificaciones;

public interface IEspecificacionMovimientoPieza
{
    TipoPieza Tipo { get; }

    IEnumerable<Movimiento> MovimientosCandidatos(Tablero tablero, Posicion origen, Color color, ContextoMovimiento contexto);
}
