using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Infrastructure.IA;

internal sealed class BuscadorMinimax(IValidadorMovimientos validador)
{
    public Movimiento? BuscarMejorMovimiento(Tablero tablero, Color turno, ContextoMovimiento contexto, int profundidad)
    {
        var candidatos = validador.TodosLosMovimientosLegales(tablero, turno, contexto).ToList();
        if (candidatos.Count == 0)
            return null;

        Movimiento? mejor = null;
        int mejorPuntaje = turno == Color.Blanco ? int.MinValue : int.MaxValue;

        foreach (var movimiento in candidatos)
        {
            var tableroResultante = tablero.AplicarMovimiento(movimiento);
            var contextoResultante = ActualizadorContexto.Actualizar(tablero, contexto, movimiento);

            int puntaje = MinimaxAlfaBeta(
                tableroResultante, turno.Opuesto(), contextoResultante, profundidad - 1, int.MinValue, int.MaxValue);

            bool esMejor = turno == Color.Blanco ? puntaje > mejorPuntaje : puntaje < mejorPuntaje;
            if (esMejor || mejor is null)
            {
                mejorPuntaje = puntaje;
                mejor = movimiento;
            }
        }

        return mejor;
    }

    private int MinimaxAlfaBeta(Tablero tablero, Color turno, ContextoMovimiento contexto, int profundidad, int alfa, int beta)
    {
        var candidatos = validador.TodosLosMovimientosLegales(tablero, turno, contexto).ToList();

        if (candidatos.Count == 0)
        {
            // Sin movimientos: jaque mate (valor extremo, favorece a quien lo dio) o ahogado (neutro).
            bool enJaque = validador.ReyEnJaque(tablero, turno);
            if (!enJaque)
                return 0;

            return turno == Color.Blanco ? int.MinValue + 1 : int.MaxValue - 1;
        }

        if (profundidad == 0)
            return EvaluadorTablero.Evaluar(tablero);

        if (turno == Color.Blanco)
        {
            int mejor = int.MinValue;
            foreach (var movimiento in candidatos)
            {
                var contextoResultante = ActualizadorContexto.Actualizar(tablero, contexto, movimiento);
                int puntaje = MinimaxAlfaBeta(tablero.AplicarMovimiento(movimiento), Color.Negro, contextoResultante, profundidad - 1, alfa, beta);
                mejor = Math.Max(mejor, puntaje);
                alfa = Math.Max(alfa, mejor);
                if (beta <= alfa)
                    break;
            }

            return mejor;
        }
        else
        {
            int mejor = int.MaxValue;
            foreach (var movimiento in candidatos)
            {
                var contextoResultante = ActualizadorContexto.Actualizar(tablero, contexto, movimiento);
                int puntaje = MinimaxAlfaBeta(tablero.AplicarMovimiento(movimiento), Color.Blanco, contextoResultante, profundidad - 1, alfa, beta);
                mejor = Math.Min(mejor, puntaje);
                beta = Math.Min(beta, mejor);
                if (beta <= alfa)
                    break;
            }

            return mejor;
        }
    }
}
