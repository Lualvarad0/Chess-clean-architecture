using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.ValueObjects;

public readonly record struct DerechosEnroque(
    bool BlancoEnroqueCorto,
    bool BlancoEnroqueLargo,
    bool NegroEnroqueCorto,
    bool NegroEnroqueLargo)
{
    public static DerechosEnroque Iniciales() => new(true, true, true, true);

    public static DerechosEnroque Ninguno() => new(false, false, false, false);

    public bool TieneEnroqueCorto(Color color) => color == Color.Blanco ? BlancoEnroqueCorto : NegroEnroqueCorto;

    public bool TieneEnroqueLargo(Color color) => color == Color.Blanco ? BlancoEnroqueLargo : NegroEnroqueLargo;

    public DerechosEnroque RevocarAmbos(Color color) => color == Color.Blanco
        ? this with { BlancoEnroqueCorto = false, BlancoEnroqueLargo = false }
        : this with { NegroEnroqueCorto = false, NegroEnroqueLargo = false };

    public DerechosEnroque RevocarCorto(Color color) => color == Color.Blanco
        ? this with { BlancoEnroqueCorto = false }
        : this with { NegroEnroqueCorto = false };

    public DerechosEnroque RevocarLargo(Color color) => color == Color.Blanco
        ? this with { BlancoEnroqueLargo = false }
        : this with { NegroEnroqueLargo = false };
}
