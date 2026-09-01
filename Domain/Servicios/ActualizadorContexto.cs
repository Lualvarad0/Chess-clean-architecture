using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

// Extraído de Partida porque el motor de búsqueda del bot (Infrastructure) necesita la misma
// lógica de casilla al paso / derechos de enroque al simular jugadas varios plies hacia adelante,
// y duplicarla ahí habría arriesgado que las dos copias se desincronizaran con el tiempo.
public static class ActualizadorContexto
{
    public static ContextoMovimiento Actualizar(Tablero tableroAntesDelMovimiento, ContextoMovimiento contextoAntes, Movimiento movimiento)
    {
        var piezaMovida = tableroAntesDelMovimiento.ObtenerPiezaEn(movimiento.Origen)!.Value;

        var casillaAlPaso = CalcularCasillaAlPaso(movimiento, piezaMovida.Tipo == TipoPieza.Peon);
        var derechosEnroque = ActualizarDerechosEnroque(contextoAntes.DerechosEnroque, piezaMovida, movimiento);

        return new ContextoMovimiento(casillaAlPaso, derechosEnroque);
    }

    private static Posicion? CalcularCasillaAlPaso(Movimiento movimiento, bool eraPeon)
    {
        if (!eraPeon || Math.Abs(movimiento.Destino.Fila - movimiento.Origen.Fila) != 2)
            return null;

        int filaIntermedia = (movimiento.Origen.Fila + movimiento.Destino.Fila) / 2;
        return Posicion.Crear(filaIntermedia, movimiento.Origen.Columna);
    }

    private static DerechosEnroque ActualizarDerechosEnroque(DerechosEnroque derechos, Pieza piezaMovida, Movimiento movimiento)
    {
        if (piezaMovida.Tipo == TipoPieza.Rey)
            derechos = derechos.RevocarAmbos(piezaMovida.Color);

        derechos = RevocarSiEsCasillaDeTorre(derechos, movimiento.Origen);
        derechos = RevocarSiEsCasillaDeTorre(derechos, movimiento.Destino);

        return derechos;
    }

    // Se revisa tanto el origen (la torre se movió) como el destino (la torre fue capturada ahí).
    private static DerechosEnroque RevocarSiEsCasillaDeTorre(DerechosEnroque derechos, Posicion posicion)
    {
        if (posicion == Posicion.Crear(0, 0)) return derechos.RevocarLargo(Color.Blanco);
        if (posicion == Posicion.Crear(0, 7)) return derechos.RevocarCorto(Color.Blanco);
        if (posicion == Posicion.Crear(7, 0)) return derechos.RevocarLargo(Color.Negro);
        if (posicion == Posicion.Crear(7, 7)) return derechos.RevocarCorto(Color.Negro);
        return derechos;
    }
}
