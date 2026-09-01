using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.ValueObjects;

public readonly record struct Pieza(TipoPieza Tipo, Color Color)
{
    public override string ToString() => $"{Color} {Tipo}";
}
