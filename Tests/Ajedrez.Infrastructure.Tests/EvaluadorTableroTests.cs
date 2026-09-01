using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.IA;
using Xunit;

namespace Ajedrez.Infrastructure.Tests;

public class EvaluadorTableroTests
{
    [Fact]
    public void PosicionInicial_estaBalanceada()
    {
        var puntaje = EvaluadorTablero.Evaluar(Tablero.PosicionInicial());

        Assert.Equal(0, puntaje);
    }

    [Fact]
    public void FaltaDamaNegra_favoreceAlBlanco()
    {
        var tablero = Tablero.Vacio()
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e1"), new Pieza(TipoPieza.Rey, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("d1"), new Pieza(TipoPieza.Dama, Color.Blanco))
            .ConPieza(Posicion.DesdeNotacionAlgebraica("e8"), new Pieza(TipoPieza.Rey, Color.Negro));

        var puntaje = EvaluadorTablero.Evaluar(tablero);

        Assert.True(puntaje > 800); // el valor de una dama, aproximadamente
    }
}
