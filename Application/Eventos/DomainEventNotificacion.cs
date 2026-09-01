using Ajedrez.Domain.SeedWork;
using MediatR;

namespace Ajedrez.Application.Eventos;

public sealed class DomainEventNotificacion<TEvento>(TEvento eventoDominio) : INotification
    where TEvento : IDomainEvent
{
    public TEvento EventoDominio { get; } = eventoDominio;
}
