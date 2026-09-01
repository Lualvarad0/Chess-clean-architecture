using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Domain.Eventos;

public sealed record PartidaIniciadaEvent(Guid PartidaId) : IDomainEvent;
