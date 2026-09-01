using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class PosicionTests
{
    [Theory]
    [InlineData("a1", 0, 0)]
    [InlineData("h8", 7, 7)]
    [InlineData("e4", 3, 4)]
    public void DesdeNotacionAlgebraica_convierteCorrectamente(string notacion, int filaEsperada, int columnaEsperada)
    {
        var posicion = Posicion.DesdeNotacionAlgebraica(notacion);

        Assert.Equal(filaEsperada, posicion.Fila);
        Assert.Equal(columnaEsperada, posicion.Columna);
    }

    [Fact]
    public void ANotacionAlgebraica_esInversaDeDesdeNotacion()
    {
        var posicion = Posicion.Crear(3, 4);

        Assert.Equal("e4", posicion.ANotacionAlgebraica());
    }

    [Fact]
    public void Crear_fueraDeRango_lanzaExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Posicion.Crear(8, 0));
    }

    [Fact]
    public void TryDesplazar_fueraDelTablero_devuelveFalse()
    {
        var origen = Posicion.Crear(0, 0);

        bool resultado = Posicion.TryDesplazar(origen, -1, 0, out _);

        Assert.False(resultado);
    }
}
