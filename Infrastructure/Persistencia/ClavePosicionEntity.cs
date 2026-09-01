namespace Ajedrez.Infrastructure.Persistencia;

// Una fila por cada posición ya alcanzada en la partida (misma clave que genera
// DetectorFinDePartida.ClavePosicion), usada para detectar repetición triple.
public sealed class ClavePosicionEntity
{
    public int Id { get; set; }
    public Guid PartidaId { get; set; }
    public int Orden { get; set; }
    public string Clave { get; set; } = string.Empty;
}
