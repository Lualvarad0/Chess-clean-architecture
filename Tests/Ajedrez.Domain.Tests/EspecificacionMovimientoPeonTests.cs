using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Especificaciones;
using Ajedrez.Domain.ValueObjects;
using Xunit;

namespace Ajedrez.Domain.Tests;

public class EspecificacionMovimientoPeonTests
{
    [Fact]
    public void PeonEnSeptimaFila_generaCuatroPromociones()
    {
        var origen = Posicion.DesdeNotacionAlgebraica("a7");
        var tablero = Tablero.Vacio().ConPieza(origen, new Pieza(TipoPieza.Peon, Color.Blanco));
        var especificacion = new EspecificacionMovimientoPeon();

        var candidatos = especificacion.MovimientosCandidatos(tablero, origen, Color.Blanco, ContextoMovimiento.Ninguno).ToList();

        Assert.Equal(4, candidatos.Count);
        Assert.Contains(candidatos, m => m.PromocionA == TipoPieza.Dama);
        Assert.Contains(candidatos, m => m.PromocionA == TipoPieza.Torre);
        Assert.Contains(candidatos, m => m.PromocionA == TipoPieza.Alfil);
        Assert.Contains(candidatos, m => m.PromocionA == TipoPieza.Caballo);
        Assert.All(candidatos, m => Assert.Equal(Posicion.DesdeNotacionAlgebraica("a8"), m.Destino));
    }

    [Fact]
    public void PeonEnFilaInicial_puedeAvanzarUnaODosCasillas()
    {
        var origen = Posicion.DesdeNotacionAlgebraica("e2");
        var tablero = Tablero.Vacio().ConPieza(origen, new Pieza(TipoPieza.Peon, Color.Blanco));
        var especificacion = new EspecificacionMovimientoPeon();

        var destinos = especificacion.MovimientosCandidatos(tablero, origen, Color.Blanco, ContextoMovimiento.Ninguno)
            .Select(m => m.Destino)
            .ToList();

        Assert.Contains(Posicion.DesdeNotacionAlgebraica("e3"), destinos);
        Assert.Contains(Posicion.DesdeNotacionAlgebraica("e4"), destinos);
        Assert.Equal(2, destinos.Count);
    }
}
