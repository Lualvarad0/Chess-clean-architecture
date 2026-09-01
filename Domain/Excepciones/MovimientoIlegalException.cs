using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Excepciones;

public sealed class MovimientoIlegalException : Exception
{
    public Movimiento Movimiento { get; }

    public MovimientoIlegalException(Movimiento movimiento)
        : base($"El movimiento {movimiento} no es legal en la posición actual.")
    {
        Movimiento = movimiento;
    }
}
