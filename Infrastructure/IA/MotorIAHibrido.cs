using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Ajedrez.Infrastructure.IA;

// Rival híbrido: mientras la partida siga una línea conocida usa el libro de aperturas
// (EntradasLibroApertura), ponderando la elección por su historial de resultados; en cuanto la
// posición se sale del libro, cae a minimax + poda alfa-beta con la profundidad del nivel.
public sealed class MotorIAHibrido(AjedrezDbContext baseDatos) : IMotorIA
{
    private static readonly IValidadorMovimientos Validador = new ValidadorMovimientos();
    private static readonly IDetectorFinDePartida DetectorFin = new DetectorFinDePartida();

    public async Task<Movimiento> ObtenerMejorMovimientoAsync(Partida partida, NivelDificultad nivel, CancellationToken cancellationToken)
    {
        var movimientoDelLibro = await IntentarObtenerMovimientoDelLibroAsync(partida, nivel, cancellationToken);
        if (movimientoDelLibro is { } movimiento)
            return movimiento;

        var contextoMovimiento = new ContextoMovimiento(partida.CasillaAlPaso, partida.DerechosEnroque);

        // Fácil no debe "pensar": fuera del libro juega un movimiento legal al azar, sin evaluar
        // nada. Con minimax, incluso a poca profundidad, el bot evita blunders obvios y termina
        // siendo demasiado fuerte para alguien que recién aprende; al azar sí se le puede ganar.
        if (nivel == NivelDificultad.Facil)
        {
            var legales = Validador.TodosLosMovimientosLegales(partida.Tablero, partida.TurnoActual, contextoMovimiento).ToList();
            return legales.Count > 0
                ? legales[Random.Shared.Next(legales.Count)]
                : throw new InvalidOperationException("No hay movimientos legales disponibles para el bot.");
        }

        var buscador = new BuscadorMinimax(Validador);
        var mejor = buscador.BuscarMejorMovimiento(partida.Tablero, partida.TurnoActual, contextoMovimiento, ProfundidadPara(nivel));

        return mejor ?? throw new InvalidOperationException("No hay movimientos legales disponibles para el bot.");
    }

    private async Task<Movimiento?> IntentarObtenerMovimientoDelLibroAsync(Partida partida, NivelDificultad nivel, CancellationToken cancellationToken)
    {
        var contextoMovimiento = new ContextoMovimiento(partida.CasillaAlPaso, partida.DerechosEnroque);
        var clave = DetectorFin.ClavePosicion(partida.Tablero, partida.TurnoActual, contextoMovimiento);

        var entradas = await baseDatos.EntradasLibroApertura
            .Where(e => e.ClavePosicion == clave)
            .ToListAsync(cancellationToken);

        if (entradas.Count == 0)
            return null;

        var elegida = ElegirEntradaDelLibro(entradas, nivel);

        return new Movimiento(
            Posicion.DesdeNotacionAlgebraica(elegida.Origen),
            Posicion.DesdeNotacionAlgebraica(elegida.Destino),
            elegida.PromocionA is null ? null : Enum.Parse<TipoPieza>(elegida.PromocionA));
    }

    private static EntradaLibroAperturaEntity ElegirEntradaDelLibro(List<EntradaLibroAperturaEntity> entradas, NivelDificultad nivel)
    {
        if (nivel == NivelDificultad.Profesional)
            return entradas.OrderByDescending(PuntajeDe).ThenByDescending(e => e.VecesJugado).First();

        if (nivel == NivelDificultad.Facil)
            return entradas[Random.Shared.Next(entradas.Count)];

        return ElegirPorSoftmax(entradas);
    }

    private static double PuntajeDe(EntradaLibroAperturaEntity entrada) => entrada.Victorias - entrada.Derrotas + 0.5 * entrada.Tablas;

    // Sorteo ponderado (softmax): favorece las líneas con mejor historial sin ser 100% predecible.
    private static EntradaLibroAperturaEntity ElegirPorSoftmax(List<EntradaLibroAperturaEntity> entradas)
    {
        var pesos = entradas.Select(e => Math.Exp(PuntajeDe(e))).ToList();
        double total = pesos.Sum();
        double sorteo = Random.Shared.NextDouble() * total;
        double acumulado = 0;

        for (int i = 0; i < entradas.Count; i++)
        {
            acumulado += pesos[i];
            if (sorteo <= acumulado)
                return entradas[i];
        }

        return entradas[^1];
    }

    private static int ProfundidadPara(NivelDificultad nivel) => nivel switch
    {
        NivelDificultad.Facil => 2,
        NivelDificultad.Dificil => 3,
        NivelDificultad.Profesional => 4,
        _ => 2
    };
}
