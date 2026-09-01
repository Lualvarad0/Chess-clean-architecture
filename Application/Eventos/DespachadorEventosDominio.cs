using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.SeedWork;
using MediatR;

namespace Ajedrez.Application.Eventos;

// Los IDomainEvent del Domain no conocen MediatR (el Domain no depende de nada externo).
// Aquí, en Application, se envuelve cada evento en una notificación genérica para poder
// publicarlo con IPublisher y que los INotificationHandler<DomainEventNotificacion<T>> lo reciban.
public sealed class DespachadorEventosDominio(IPublisher publisher) : IDespachadorEventosDominio
{
    public async Task DespacharYLimpiarAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        var eventos = aggregateRoot.EventosDominio.ToList();
        aggregateRoot.LimpiarEventos();

        foreach (var evento in eventos)
        {
            var tipoNotificacion = typeof(DomainEventNotificacion<>).MakeGenericType(evento.GetType());
            var notificacion = (INotification)Activator.CreateInstance(tipoNotificacion, evento)!;
            await publisher.Publish(notificacion, cancellationToken);
        }
    }
}
