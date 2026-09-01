using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoTorre : EspecificacionMovimientoDeslizante
{
    public override TipoPieza Tipo => TipoPieza.Torre;

    protected override (int DeltaFila, int DeltaColumna)[] Direcciones =>
    [
        (1, 0), (-1, 0), (0, 1), (0, -1)
    ];
}
