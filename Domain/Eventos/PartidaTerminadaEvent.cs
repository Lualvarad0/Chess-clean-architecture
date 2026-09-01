using Ajedrez.Domain.SeedWork;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Eventos;

public sealed record PartidaTerminadaEvent(Guid PartidaId, ResultadoPartida Resultado) : IDomainEvent;
