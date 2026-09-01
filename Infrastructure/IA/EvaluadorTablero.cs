using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;

namespace Ajedrez.Infrastructure.IA;

// Evaluación puramente material (sin tablas de posición todavía). Convención estándar de
// motores: positivo favorece a Blanco, negativo favorece a Negro.
internal static class EvaluadorTablero
{
    private static readonly Dictionary<TipoPieza, int> ValorPieza = new()
    {
        [TipoPieza.Peon] = 100,
        [TipoPieza.Caballo] = 320,
        [TipoPieza.Alfil] = 330,
        [TipoPieza.Torre] = 500,
        [TipoPieza.Dama] = 900,
        [TipoPieza.Rey] = 20000
    };

    public static int Evaluar(Tablero tablero)
    {
        int puntaje = 0;

        foreach (var color in new[] { Color.Blanco, Color.Negro })
        {
            int signo = color == Color.Blanco ? 1 : -1;
            foreach (var (_, pieza) in tablero.PiezasDeColor(color))
                puntaje += signo * ValorPieza[pieza.Tipo];
        }

        return puntaje;
    }
}
