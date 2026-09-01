using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.Especificaciones;

public sealed class EspecificacionMovimientoAlfil : EspecificacionMovimientoDeslizante
{
    public override TipoPieza Tipo => TipoPieza.Alfil;

    protected override (int DeltaFila, int DeltaColumna)[] Direcciones =>
    [
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    ];
}
