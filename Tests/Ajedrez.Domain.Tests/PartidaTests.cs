using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Excepciones;
using Ajedrez.Domain.Eventos;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class PartidaTests
{
    private readonly IValidadorMovimientos _validador = new ValidadorMovimientos();
    private readonly IDetectorFinDePartida _detectorFin = new DetectorFinDePartida();

    private static Movimiento Mov(string origen, string destino) =>
        new(Posicion.DesdeNotacionAlgebraica(origen), Posicion.DesdeNotacionAlgebraica(destino));

    [Fact]
    public void JaqueMatePastor_terminaLaPartidaConGanadorNegro()
    {
        var partida = Partida.Iniciar();

        partida.RealizarMovimiento(Mov("f2", "f3"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("e7", "e5"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("g2", "g4"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("d8", "h4"), _validador, _detectorFin);

        Assert.Equal(EstadoPartida.Finalizada, partida.Estado);
        Assert.NotNull(partida.Resultado);
        Assert.Equal(MotivoFinalizacion.JaqueMate, partida.Resultado.Value.Motivo);
        Assert.NotNull(partida.Resultado.Value.Ganador);
        Assert.Equal(Color.Negro, partida.Resultado.Value.Ganador!.Value);
        Assert.Contains(partida.EventosDominio, e => e is JaqueMateDetectadoEvent);
        Assert.Contains(partida.EventosDominio, e => e is PartidaTerminadaEvent);
    }

    [Fact]
    public void MovimientoIlegal_lanzaExcepcion()
    {
        var partida = Partida.Iniciar();

        Assert.Throws<MovimientoIlegalException>(() =>
            partida.RealizarMovimiento(Mov("e2", "e5"), _validador, _detectorFin));
    }

    [Fact]
    public void CapturaAlPaso_eliminaElPeonCapturado()
    {
        var partida = Partida.Iniciar();

        partida.RealizarMovimiento(Mov("e2", "e4"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("a7", "a6"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("e4", "e5"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("d7", "d5"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("e5", "d6"), _validador, _detectorFin); // captura al paso

        Assert.Null(partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("d5")));
        Assert.Null(partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("e5")));

        var piezaEnD6 = partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("d6"));
        Assert.NotNull(piezaEnD6);
        Assert.Equal(new Pieza(TipoPieza.Peon, Color.Blanco), piezaEnD6!.Value);
    }

    [Fact]
    public void EnroqueCorto_mueveElReyYLaTorreALaVez()
    {
        var partida = Partida.Iniciar();

        partida.RealizarMovimiento(Mov("e2", "e4"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("e7", "e5"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("g1", "f3"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("b8", "c6"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("f1", "c4"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("g8", "f6"), _validador, _detectorFin);
        partida.RealizarMovimiento(Mov("e1", "g1"), _validador, _detectorFin); // enroque corto

        var piezaEnG1 = partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("g1"));
        var piezaEnF1 = partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("f1"));

        Assert.NotNull(piezaEnG1);
        Assert.NotNull(piezaEnF1);
        Assert.Equal(new Pieza(TipoPieza.Rey, Color.Blanco), piezaEnG1!.Value);
        Assert.Equal(new Pieza(TipoPieza.Torre, Color.Blanco), piezaEnF1!.Value);
        Assert.Null(partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("e1")));
        Assert.Null(partida.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("h1")));
    }
}
