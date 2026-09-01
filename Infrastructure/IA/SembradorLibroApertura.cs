using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.Persistencia;

namespace Ajedrez.Infrastructure.IA;

// Genera las entradas iniciales del libro de aperturas jugando cada línea conocida sobre el
// motor real (Partida + ValidadorMovimientos). Esto es deliberado: si alguna de las jugadas de
// abajo estuviera mal escrita, RealizarMovimiento lanzaría MovimientoIlegalException al sembrar
// en vez de guardar silenciosamente una entrada corrupta en el libro.
public static class SembradorLibroApertura
{
    private static readonly (string Nombre, string[] Movimientos)[] Lineas =
    [
        ("Apertura Italiana", ["e2e4", "e7e5", "g1f3", "b8c6", "f1c4", "g8f6"]),
        ("Ruy López", ["e2e4", "e7e5", "g1f3", "b8c6", "f1b5", "a7a6"]),
        ("Gambito Evans", ["e2e4", "e7e5", "g1f3", "b8c6", "f1c4", "f8c5", "b2b4"]),
        ("Ataque Vienés", ["e2e4", "e7e5", "b1c3"]),
        ("Defensa Siciliana Najdorf", ["e2e4", "c7c5", "g1f3", "d7d6", "d2d4", "c5d4", "f3d4", "g8f6", "b1c3", "a7a6"]),
        ("Defensa Francesa", ["e2e4", "e7e6", "d2d4", "d7d5"]),
        ("Defensa Caro-Kann", ["e2e4", "c7c6", "d2d4", "d7d5"]),
        ("Defensa Escandinava", ["e2e4", "d7d5"]),
        ("Defensa Pirc", ["e2e4", "d7d6", "d2d4", "g8f6"]),
        ("Gambito de Dama Aceptado", ["d2d4", "d7d5", "c2c4", "d5c4"]),
        ("Gambito de Dama Rehusado", ["d2d4", "d7d5", "c2c4", "e7e6"]),
        ("Defensa India de Rey", ["d2d4", "g8f6", "c2c4", "g7g6"]),
        ("Defensa Nimzoindia", ["d2d4", "g8f6", "c2c4", "e7e6", "b1c3", "f8b4"]),
        ("Apertura Inglesa", ["c2c4", "e7e5"]),
        ("Apertura Réti", ["g1f3", "d7d5", "c2c4"])
    ];

    public static List<EntradaLibroAperturaEntity> GenerarEntradas()
    {
        var validador = new ValidadorMovimientos();
        var detectorFin = new DetectorFinDePartida();
        var entradasUnicas = new Dictionary<(string Clave, string Origen, string Destino), EntradaLibroAperturaEntity>();

        foreach (var (_, movimientos) in Lineas)
        {
            var partida = Partida.Iniciar();

            foreach (var notacion in movimientos)
            {
                var origen = Posicion.DesdeNotacionAlgebraica(notacion[..2]);
                var destino = Posicion.DesdeNotacionAlgebraica(notacion[2..4]);

                var contexto = new ContextoMovimiento(partida.CasillaAlPaso, partida.DerechosEnroque);
                var clavePosicion = detectorFin.ClavePosicion(partida.Tablero, partida.TurnoActual, contexto);

                var llave = (clavePosicion, origen.ANotacionAlgebraica(), destino.ANotacionAlgebraica());
                entradasUnicas.TryAdd(llave, new EntradaLibroAperturaEntity
                {
                    ClavePosicion = clavePosicion,
                    Origen = origen.ANotacionAlgebraica(),
                    Destino = destino.ANotacionAlgebraica(),
                    PromocionA = null
                });

                partida.RealizarMovimiento(new Movimiento(origen, destino), validador, detectorFin);
            }
        }

        return entradasUnicas.Values.ToList();
    }
}
