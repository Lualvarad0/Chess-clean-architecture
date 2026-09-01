using Ajedrez.Application.Eventos;
using Ajedrez.Domain.Eventos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ajedrez.Application.EventHandlers;

public sealed class PartidaTerminadaEventHandler(ILogger<PartidaTerminadaEventHandler> logger)
    : INotificationHandler<DomainEventNotificacion<PartidaTerminadaEvent>>
{
    public Task Handle(DomainEventNotificacion<PartidaTerminadaEvent> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Partida {PartidaId} finalizada. Motivo: {Motivo}. Ganador: {Ganador}",
            notification.EventoDominio.PartidaId,
            notification.EventoDominio.Resultado.Motivo,
            notification.EventoDominio.Resultado.Ganador?.ToString() ?? "Ninguno");

        return Task.CompletedTask;
    }
}
