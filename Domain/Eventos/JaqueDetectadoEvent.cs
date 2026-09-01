using Ajedrez.Domain.Enums;
using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Domain.Eventos;

public sealed record JaqueDetectadoEvent(Guid PartidaId, Color ColorEnJaque) : IDomainEvent;
