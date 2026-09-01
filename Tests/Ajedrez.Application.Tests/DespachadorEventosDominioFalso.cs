using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.SeedWork;

namespace Ajedrez.Application.Tests;

public sealed class DespachadorEventosDominioFalso : IDespachadorEventosDominio
{
    public Task DespacharYLimpiarAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        aggregateRoot.LimpiarEventos();
        return Task.CompletedTask;
    }
}
