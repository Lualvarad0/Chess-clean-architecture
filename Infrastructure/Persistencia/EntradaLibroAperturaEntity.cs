namespace Ajedrez.Infrastructure.Persistencia;

// Cada fila es "en esta posición (ClavePosicion), esta jugada es una opción de libro",
// con contadores que se van actualizando después de cada partida jugada (ver
// PartidaTerminadaActualizarLibroHandler) para que el libro se sesgue hacia lo que funciona.
public sealed class EntradaLibroAperturaEntity
{
    public int Id { get; set; }
    public string ClavePosicion { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public string? PromocionA { get; set; }
    public int VecesJugado { get; set; }
    public int Victorias { get; set; }
    public int Derrotas { get; set; }
    public int Tablas { get; set; }
}
