using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Entidades;

public sealed class Tablero
{
    private readonly Pieza?[,] _casillas;

    private Tablero(Pieza?[,] casillas)
    {
        _casillas = casillas;
    }

    public Pieza? ObtenerPiezaEn(Posicion posicion) => _casillas[posicion.Fila, posicion.Columna];

    public bool EstaVacia(Posicion posicion) => ObtenerPiezaEn(posicion) is null;

    public IEnumerable<(Posicion Posicion, Pieza Pieza)> PiezasDeColor(Color color)
    {
        for (int fila = 0; fila < 8; fila++)
        {
            for (int columna = 0; columna < 8; columna++)
            {
                if (_casillas[fila, columna] is { } pieza && pieza.Color == color)
                    yield return (Posicion.Crear(fila, columna), pieza);
            }
        }
    }

    public Posicion PosicionDelRey(Color color)
    {
        foreach (var (posicion, pieza) in PiezasDeColor(color))
        {
            if (pieza.Tipo == TipoPieza.Rey)
                return posicion;
        }

        throw new InvalidOperationException($"No hay rey {color} en el tablero.");
    }

    public Tablero AplicarMovimiento(Movimiento movimiento)
    {
        var pieza = ObtenerPiezaEn(movimiento.Origen)
            ?? throw new InvalidOperationException($"No hay pieza en {movimiento.Origen}.");

        var nuevasCasillas = (Pieza?[,])_casillas.Clone();

        nuevasCasillas[movimiento.Origen.Fila, movimiento.Origen.Columna] = null;

        var piezaFinal = movimiento.PromocionA is { } tipoPromocion ? pieza with { Tipo = tipoPromocion } : pieza;
        nuevasCasillas[movimiento.Destino.Fila, movimiento.Destino.Columna] = piezaFinal;

        if (movimiento.EsCapturaAlPaso)
        {
            // El peón capturado al paso queda en la misma fila que la casilla de origen,
            // en la columna de destino (nunca es la propia casilla de destino).
            nuevasCasillas[movimiento.Origen.Fila, movimiento.Destino.Columna] = null;
        }

        if (movimiento.EsEnroqueCorto || movimiento.EsEnroqueLargo)
        {
            int fila = movimiento.Origen.Fila;
            int columnaTorreOrigen = movimiento.EsEnroqueCorto ? 7 : 0;
            int columnaTorreDestino = movimiento.EsEnroqueCorto ? 5 : 3;

            nuevasCasillas[fila, columnaTorreDestino] = nuevasCasillas[fila, columnaTorreOrigen];
            nuevasCasillas[fila, columnaTorreOrigen] = null;
        }

        return new Tablero(nuevasCasillas);
    }

    public static Tablero PosicionInicial()
    {
        var casillas = new Pieza?[8, 8];

        for (int columna = 0; columna < 8; columna++)
        {
            casillas[1, columna] = new Pieza(TipoPieza.Peon, Color.Blanco);
            casillas[6, columna] = new Pieza(TipoPieza.Peon, Color.Negro);
        }

        ColocarFilaBase(casillas, 0, Color.Blanco);
        ColocarFilaBase(casillas, 7, Color.Negro);

        return new Tablero(casillas);
    }

    public static Tablero Vacio() => new(new Pieza?[8, 8]);

    public Tablero ConPieza(Posicion posicion, Pieza pieza)
    {
        var nuevasCasillas = (Pieza?[,])_casillas.Clone();
        nuevasCasillas[posicion.Fila, posicion.Columna] = pieza;
        return new Tablero(nuevasCasillas);
    }

    private static void ColocarFilaBase(Pieza?[,] casillas, int fila, Color color)
    {
        TipoPieza[] orden =
        [
            TipoPieza.Torre, TipoPieza.Caballo, TipoPieza.Alfil, TipoPieza.Dama,
            TipoPieza.Rey, TipoPieza.Alfil, TipoPieza.Caballo, TipoPieza.Torre
        ];

        for (int columna = 0; columna < 8; columna++)
            casillas[fila, columna] = new Pieza(orden[columna], color);
    }
}
