using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Application.Interfaces;

public interface IDespachadorEventosDominio
{
    Task DespacharYLimpiarAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken);
}
