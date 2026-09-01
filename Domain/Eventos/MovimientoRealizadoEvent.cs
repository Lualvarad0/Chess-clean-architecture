using Ajedrez.Domain.SeedWork;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Eventos;

public sealed record MovimientoRealizadoEvent(Guid PartidaId, Movimiento Movimiento) : IDomainEvent;
