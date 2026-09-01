using Ajedrez.Application.Eventos;
using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Eventos;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.Persistencia;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ajedrez.Infrastructure.IA;

// El "aprendizaje" del libro: al terminar una partida, recorre su historial y por cada jugada
// que coincida con una entrada del libro, actualiza victorias/derrotas/tablas según el color que
// la jugó. No importa si esa jugada la hizo el bot o el humano — el libro aprende de ambos.
public sealed class PartidaTerminadaActualizarLibroHandler(AjedrezDbContext baseDatos, IPartidaRepository partidaRepositorio)
    : INotificationHandler<DomainEventNotificacion<PartidaTerminadaEvent>>
{
    private static readonly IDetectorFinDePartida DetectorFin = new DetectorFinDePartida();

    public async Task Handle(DomainEventNotificacion<PartidaTerminadaEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.EventoDominio;
        var partida = await partidaRepositorio.ObtenerPorIdAsync(evento.PartidaId, cancellationToken);
        if (partida is null)
            return;

        var tableroReplay = Tablero.PosicionInicial();
        var contextoReplay = new ContextoMovimiento(null, DerechosEnroque.Iniciales());
        var color = Color.Blanco;
        bool huboCambios = false;

        foreach (var movimiento in partida.Historial)
        {
            var clave = DetectorFin.ClavePosicion(tableroReplay, color, contextoReplay);
            var origen = movimiento.Origen.ANotacionAlgebraica();
            var destino = movimiento.Destino.ANotacionAlgebraica();
            var promocion = movimiento.PromocionA?.ToString();

            var entrada = await baseDatos.EntradasLibroApertura.FirstOrDefaultAsync(
                e => e.ClavePosicion == clave && e.Origen == origen && e.Destino == destino && e.PromocionA == promocion,
                cancellationToken);

            if (entrada is not null)
            {
                entrada.VecesJugado++;
                if (evento.Resultado.Ganador == color) entrada.Victorias++;
                else if (evento.Resultado.Ganador is null) entrada.Tablas++;
                else entrada.Derrotas++;
                huboCambios = true;
            }

            contextoReplay = ActualizadorContexto.Actualizar(tableroReplay, contextoReplay, movimiento);
            tableroReplay = tableroReplay.AplicarMovimiento(movimiento);
            color = color.Opuesto();
        }

        if (huboCambios)
            await baseDatos.SaveChangesAsync(cancellationToken);
    }
}
