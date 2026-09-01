using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class DetectorFinDePartidaTests
{
    private readonly ValidadorMovimientos _validador = new();
    private readonly DetectorFinDePartida _detector = new();

    [Fact]
    public void PosicionDeAhogadoClasica_seDetectaComoTablas()
    {
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("h1"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("f2"), new Pieza(TipoPieza.Rey, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("g3"), new Pieza(TipoPieza.Dama, Color.Negro));

        var resultado = _detector.Evaluar(tablero, Color.Blanco, ContextoMovimiento.Ninguno, _validador, [], 0);

        Assert.NotNull(resultado);
        Assert.Equal(MotivoFinalizacion.Ahogado, resultado.Value.Motivo);
        Assert.Null(resultado.Value.Ganador);
    }

    [Fact]
    public void SoloLosDosReyes_esMaterialInsuficiente()
    {
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e4"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e8"), new Pieza(TipoPieza.Rey, Color.Negro));

        var resultado = _detector.Evaluar(tablero, Color.Blanco, ContextoMovimiento.Ninguno, _validador, [], 0);

        Assert.NotNull(resultado);
        Assert.Equal(MotivoFinalizacion.MaterialInsuficiente, resultado.Value.Motivo);
    }
}
