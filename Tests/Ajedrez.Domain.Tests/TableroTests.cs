using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class TableroTests
{
    [Fact]
    public void PosicionInicial_tiene32Piezas()
    {
        var tablero = Tablero.PosicionInicial();

        int totalPiezas = tablero.PiezasDeColor(Color.Blanco).Count() + tablero.PiezasDeColor(Color.Negro).Count();

        Assert.Equal(32, totalPiezas);
    }

    [Fact]
    public void PosicionInicial_reyBlancoEstaEnE1()
    {
        var tablero = Tablero.PosicionInicial();

        var posicionRey = tablero.PosicionDelRey(Color.Blanco);

        Assert.Equal(Posicion.DesdeNotacionAlgebraica("e1"), posicionRey);
    }

    [Fact]
    public void AplicarMovimiento_esInmutableYDejaOrigenVacio()
    {
        var tablero = Tablero.PosicionInicial();
        var origen = Posicion.DesdeNotacionAlgebraica("e2");
        var destino = Posicion.DesdeNotacionAlgebraica("e4");

        var resultado = tablero.AplicarMovimiento(new Movimiento(origen, destino));

        Assert.True(resultado.EstaVacia(origen));

        var piezaEnDestino = resultado.ObtenerPiezaEn(destino);
        Assert.NotNull(piezaEnDestino);
        Assert.Equal(new Pieza(TipoPieza.Peon, Color.Blanco), piezaEnDestino!.Value);

        Assert.NotNull(tablero.ObtenerPiezaEn(origen)); // el tablero original no cambia
    }
}
