using Ajedrez.Domain.Enums;
using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Domain.Eventos;

public sealed record JaqueMateDetectadoEvent(Guid PartidaId, Color ColorPerdedor) : IDomainEvent;
