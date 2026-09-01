using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoDama : EspecificacionMovimientoDeslizante
{
    public override TipoPieza Tipo => TipoPieza.Dama;

    protected override (int DeltaFila, int DeltaColumna)[] Direcciones =>
    [
        (1, 0), (-1, 0), (0, 1), (0, -1),
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    ];
}
