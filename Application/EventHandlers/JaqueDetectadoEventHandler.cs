using Ajedrez.Application.Eventos;
using Ajedrez.Domain.Eventos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ajedrez.Application.EventHandlers;

public sealed class JaqueDetectadoEventHandler(ILogger<JaqueDetectadoEventHandler> logger)
    : INotificationHandler<DomainEventNotificacion<JaqueDetectadoEvent>>
{
    public Task Handle(DomainEventNotificacion<JaqueDetectadoEvent> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Jaque detectado en la partida {PartidaId} contra {Color}",
            notification.EventoDominio.PartidaId,
            notification.EventoDominio.ColorEnJaque);

        return Task.CompletedTask;
    }
}
