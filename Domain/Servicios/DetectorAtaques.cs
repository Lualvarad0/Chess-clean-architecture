using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Especificaciones;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

public static class DetectorAtaques
{
    public static bool EsCasillaAtacada(Tablero tablero, Posicion posicion, Color colorAtacante, ProveedorEspecificaciones especificaciones)
    {
        foreach (var (origen, pieza) in tablero.PiezasDeColor(colorAtacante))
        {
            if (pieza.Tipo == TipoPieza.Peon)
            {
                if (AtacaComoPeon(origen, posicion, colorAtacante))
                    return true;

                continue;
            }

            // ContextoMovimiento.Ninguno evita que el rey genere candidatos de enroque aquí:
            // el enroque no es una amenaza de captura, así que no cuenta como "ataque".
            var especificacion = especificaciones.Para(pieza.Tipo);
            var candidatos = especificacion.MovimientosCandidatos(tablero, origen, colorAtacante, ContextoMovimiento.Ninguno);

            foreach (var movimiento in candidatos)
            {
                if (movimiento.Destino == posicion)
                    return true;
            }
        }

        return false;
    }

    private static bool AtacaComoPeon(Posicion origenPeon, Posicion posicion, Color colorPeon)
    {
        int direccion = colorPeon == Color.Blanco ? 1 : -1;
        return posicion.Fila == origenPeon.Fila + direccion && Math.Abs(posicion.Columna - origenPeon.Columna) == 1;
    }
}
