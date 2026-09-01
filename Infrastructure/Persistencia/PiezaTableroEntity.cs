namespace Ajedrez.Infrastructure.Persistencia;

// Una fila por cada pieza actualmente en el tablero de una partida.
public sealed class PiezaTableroEntity
{
    public int Id { get; set; }
    public Guid PartidaId { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
