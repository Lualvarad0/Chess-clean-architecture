namespace Ajedrez.Domain.ValueObjects;

public readonly record struct ContextoMovimiento(Posicion? CasillaAlPaso, DerechosEnroque DerechosEnroque)
{
    public static ContextoMovimiento Ninguno { get; } = new(null, DerechosEnroque.Ninguno());
}
