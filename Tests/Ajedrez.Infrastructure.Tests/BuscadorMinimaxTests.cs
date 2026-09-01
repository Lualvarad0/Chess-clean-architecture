using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.IA;
using Xunit;

namespace Ajedrez.Infrastructure.Tests;

public class BuscadorMinimaxTests
{
    [Fact]
    public void BuscarMejorMovimiento_encuentraMateEnUno()
    {
        // Rey negro atrapado detrás de sus propios peones; Torre e1-e8 es mate.
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("g8"), new Pieza(TipoPieza.Rey, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("f7"), new Pieza(TipoPieza.Peon, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("g7"), new Pieza(TipoPieza.Peon, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("h7"), new Pieza(TipoPieza.Peon, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("a1"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e1"), new Pieza(TipoPieza.Torre, Color.Blanco));

        var buscador = new BuscadorMinimax(new ValidadorMovimientos());
        var movimiento = buscador.BuscarMejorMovimiento(tablero, Color.Blanco, ContextoMovimiento.Ninguno, profundidad: 1);

        Assert.NotNull(movimiento);
        Assert.Equal(Posicion.DesdeNotacionAlgebraica("e1"), movimiento!.Value.Origen);
        Assert.Equal(Posicion.DesdeNotacionAlgebraica("e8"), movimiento.Value.Destino);
    }

    [Fact]
    public void BuscarMejorMovimiento_capturaUnaDamaGratis()
    {
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e1"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("a4"), new Pieza(TipoPieza.Torre, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("a8"), new Pieza(TipoPieza.Dama, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("h8"), new Pieza(TipoPieza.Rey, Color.Negro));

        var buscador = new BuscadorMinimax(new ValidadorMovimientos());
        var movimiento = buscador.BuscarMejorMovimiento(tablero, Color.Blanco, ContextoMovimiento.Ninguno, profundidad: 2);

        Assert.NotNull(movimiento);
        Assert.Equal(Posicion.DesdeNotacionAlgebraica("a4"), movimiento!.Value.Origen);
        Assert.Equal(Posicion.DesdeNotacionAlgebraica("a8"), movimiento.Value.Destino);
    }
}
