using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Especificaciones;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Domain.Servicios;

public sealed class ValidadorMovimientos : IValidadorMovimientos
{
    private readonly ProveedorEspecificaciones _especificaciones = new();

    public bool ReyEnJaque(Tablero tablero, Color color)
    {
        var posicionRey = tablero.PosicionDelRey(color);
        return DetectorAtaques.EsCasillaAtacada(tablero, posicionRey, color.Opuesto(), _especificaciones);
    }

    public IEnumerable<Movimiento> MovimientosLegales(Tablero tablero, Posicion origen, Color turno, ContextoMovimiento contexto)
    {
        if (tablero.ObtenerPiezaEn(origen) is not { } pieza || pieza.Color != turno)
            yield break;

        var candidatos = _especificaciones.Para(pieza.Tipo).MovimientosCandidatos(tablero, origen, turno, contexto);

        foreach (var movimiento in candidatos)
        {
            if (EsLegal(tablero, movimiento, turno))
                yield return movimiento;
        }
    }

    public IEnumerable<Movimiento> TodosLosMovimientosLegales(Tablero tablero, Color turno, ContextoMovimiento contexto)
    {
        foreach (var (posicion, _) in tablero.PiezasDeColor(turno))
        {
            foreach (var movimiento in MovimientosLegales(tablero, posicion, turno, contexto))
                yield return movimiento;
        }
    }

    private bool EsLegal(Tablero tablero, Movimiento movimiento, Color turno)
    {
        if ((movimiento.EsEnroqueCorto || movimiento.EsEnroqueLargo) && !EnroqueEsLegal(tablero, movimiento, turno))
            return false;

        var tableroResultante = tablero.AplicarMovimiento(movimiento);
        return !ReyEnJaque(tableroResultante, turno);
    }

    // Además de que el rey no termine en jaque (verificado en EsLegal), el enroque exige
    // que el rey no esté en jaque al iniciar ni pase por una casilla atacada.
    private bool EnroqueEsLegal(Tablero tablero, Movimiento movimiento, Color turno)
    {
        if (ReyEnJaque(tablero, turno))
            return false;

        int fila = movimiento.Origen.Fila;
        int columnaIntermedia = movimiento.EsEnroqueCorto ? 5 : 3;
        var posicionIntermedia = Posicion.Crear(fila, columnaIntermedia);

        return !DetectorAtaques.EsCasillaAtacada(tablero, posicionIntermedia, turno.Opuesto(), _especificaciones);
    }
}
