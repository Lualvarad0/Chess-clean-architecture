using Ajedrez.Domain.Enums;
using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Domain.Eventos;

public sealed record TablasDetectadasEvent(Guid PartidaId, MotivoFinalizacion Motivo) : IDomainEvent;
