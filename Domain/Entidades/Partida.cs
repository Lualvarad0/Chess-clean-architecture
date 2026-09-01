using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Eventos;
using Ajedrez.Domain.Excepciones;
using Ajedrez.Domain.SeedWork;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Entidades;

public sealed class Partida : AggregateRoot
{
    private readonly List<Movimiento> _historial = [];
    private readonly List<string> _historialClavesPosicion = [];

    public Tablero Tablero { get; private set; }
    public Color TurnoActual { get; private set; }
    public EstadoPartida Estado { get; private set; }
    public DerechosEnroque DerechosEnroque { get; private set; }
    public Posicion? CasillaAlPaso { get; private set; }
    public int ContadorMediosMovimientosSinCaptura { get; private set; }
    public ResultadoPartida? Resultado { get; private set; }
    public IReadOnlyList<Movimiento> Historial => _historial.AsReadOnly();
    public IReadOnlyList<string> HistorialClavesPosicion => _historialClavesPosicion.AsReadOnly();

    private Partida(Guid id, Tablero tablero, Color turnoActual, DerechosEnroque derechosEnroque)
    {
        Id = id;
        Tablero = tablero;
        TurnoActual = turnoActual;
        DerechosEnroque = derechosEnroque;
        Estado = EstadoPartida.EnCurso;
    }

    public static Partida Iniciar()
    {
        var partida = new Partida(Guid.NewGuid(), Tablero.PosicionInicial(), Color.Blanco, DerechosEnroque.Iniciales());
        partida.AgregarEvento(new PartidaIniciadaEvent(partida.Id));
        return partida;
    }

    // Reconstruye una partida ya existente desde el estado guardado (Infrastructure), sin volver
    // a disparar los Domain Events: esos ya ocurrieron la primera vez que se jugó cada movimiento.
    public static Partida Reconstruir(
        Guid id,
        Tablero tablero,
        Color turnoActual,
        EstadoPartida estado,
        DerechosEnroque derechosEnroque,
        Posicion? casillaAlPaso,
        int contadorMediosMovimientosSinCaptura,
        ResultadoPartida? resultado,
        IEnumerable<Movimiento> historial,
        IEnumerable<string> historialClavesPosicion)
    {
        var partida = new Partida(id, tablero, turnoActual, derechosEnroque)
        {
            Estado = estado,
            CasillaAlPaso = casillaAlPaso,
            ContadorMediosMovimientosSinCaptura = contadorMediosMovimientosSinCaptura,
            Resultado = resultado
        };

        partida._historial.AddRange(historial);
        partida._historialClavesPosicion.AddRange(historialClavesPosicion);

        return partida;
    }

    public void RealizarMovimiento(Movimiento movimientoSolicitado, IValidadorMovimientos validador, IDetectorFinDePartida detectorFin)
    {
        if (Estado == EstadoPartida.Finalizada)
            throw new InvalidOperationException("La partida ya finalizó.");

        var contexto = ContextoActual();
        var candidatos = validador.MovimientosLegales(Tablero, movimientoSolicitado.Origen, TurnoActual, contexto).ToList();
        int indice = candidatos.FindIndex(m => m.Destino == movimientoSolicitado.Destino && m.PromocionA == movimientoSolicitado.PromocionA);

        if (indice < 0)
            throw new MovimientoIlegalException(movimientoSolicitado);

        var movimiento = candidatos[indice];
        var tableroAntes = Tablero;

        bool huboCaptura = tableroAntes.ObtenerPiezaEn(movimiento.Destino) is not null || movimiento.EsCapturaAlPaso;
        bool eraPeon = tableroAntes.ObtenerPiezaEn(movimiento.Origen)!.Value.Tipo == TipoPieza.Peon;

        Tablero = tableroAntes.AplicarMovimiento(movimiento);

        var contextoActualizado = ActualizadorContexto.Actualizar(tableroAntes, contexto, movimiento);
        DerechosEnroque = contextoActualizado.DerechosEnroque;
        CasillaAlPaso = contextoActualizado.CasillaAlPaso;
        ContadorMediosMovimientosSinCaptura = huboCaptura || eraPeon ? 0 : ContadorMediosMovimientosSinCaptura + 1;

        _historial.Add(movimiento);
        TurnoActual = TurnoActual.Opuesto();

        var contextoNuevo = ContextoActual();
        _historialClavesPosicion.Add(detectorFin.ClavePosicion(Tablero, TurnoActual, contextoNuevo));

        AgregarEvento(new MovimientoRealizadoEvent(Id, movimiento));

        EvaluarEstado(validador, detectorFin, contextoNuevo);
    }

    private void EvaluarEstado(IValidadorMovimientos validador, IDetectorFinDePartida detectorFin, ContextoMovimiento contexto)
    {
        var resultado = detectorFin.Evaluar(Tablero, TurnoActual, contexto, validador, _historialClavesPosicion, ContadorMediosMovimientosSinCaptura);

        if (resultado is { } r)
        {
            Resultado = r;
            Estado = EstadoPartida.Finalizada;

            AgregarEvento(r.Motivo == MotivoFinalizacion.JaqueMate
                ? new JaqueMateDetectadoEvent(Id, TurnoActual)
                : new TablasDetectadasEvent(Id, r.Motivo));

            AgregarEvento(new PartidaTerminadaEvent(Id, r));
            return;
        }

        bool enJaque = validador.ReyEnJaque(Tablero, TurnoActual);
        Estado = enJaque ? EstadoPartida.Jaque : EstadoPartida.EnCurso;

        if (enJaque)
            AgregarEvento(new JaqueDetectadoEvent(Id, TurnoActual));
    }

    private ContextoMovimiento ContextoActual() => new(CasillaAlPaso, DerechosEnroque);
}
