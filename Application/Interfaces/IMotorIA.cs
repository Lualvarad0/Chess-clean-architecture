using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Application.Interfaces;

public interface IMotorIA
{
    Task<Movimiento> ObtenerMejorMovimientoAsync(Partida partida, NivelDificultad nivel, CancellationToken cancellationToken);
}
