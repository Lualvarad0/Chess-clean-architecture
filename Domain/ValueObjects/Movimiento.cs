using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.ValueObjects;

public readonly record struct Movimiento(
    Posicion Origen,
    Posicion Destino,
    TipoPieza? PromocionA = null,
    bool EsEnroqueCorto = false,
    bool EsEnroqueLargo = false,
    bool EsCapturaAlPaso = false)
{
    public override string ToString() => $"{Origen}-{Destino}";
}
