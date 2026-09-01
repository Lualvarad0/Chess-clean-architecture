using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class ValidadorMovimientosTests
{
    [Fact]
    public void PosicionInicial_blancoTiene20MovimientosLegales()
    {
        var tablero = Tablero.PosicionInicial();
        var validador = new ValidadorMovimientos();
        var contexto = new ContextoMovimiento(null, DerechosEnroque.Iniciales());

        var movimientos = validador.TodosLosMovimientosLegales(tablero, Color.Blanco, contexto).ToList();

        Assert.Equal(20, movimientos.Count);
    }

    [Fact]
    public void PiezaClavada_soloPuedeMoverseDentroDeLaLineaDeLaClavada()
    {
        // Rey blanco en e1, torre blanca clavada en e2 por una torre negra en e8.
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e1"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e2"), new Pieza(TipoPieza.Torre, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e8"), new Pieza(TipoPieza.Torre, Color.Negro))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("a1"), new Pieza(TipoPieza.Rey, Color.Negro));

        var validador = new ValidadorMovimientos();
        var origenTorreClavada = Posicion.DesdeNotacionAlgebraica("e2");

        var movimientos = validador
            .MovimientosLegales(tablero, origenTorreClavada, Color.Blanco, ContextoMovimiento.Ninguno)
            .ToList();

        Assert.NotEmpty(movimientos);
        Assert.All(movimientos, m => Assert.Equal(4, m.Destino.Columna));
    }
}
