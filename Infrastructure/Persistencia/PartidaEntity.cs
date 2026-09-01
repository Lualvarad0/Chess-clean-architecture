namespace Ajedrez.Infrastructure.Persistencia;

// Fila de la tabla "Partidas" — solo columnas escalares. El tablero, el historial y las claves
// de repetición viven en sus propias tablas relacionadas (PiezasTablero, Movimientos,
// ClavesPosicion), no como un blob serializado: es una base SQL, así que el modelo es relacional.
public sealed class PartidaEntity
{
    public Guid Id { get; set; }
    public string TurnoActual { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool BlancoEnroqueCorto { get; set; }
    public bool BlancoEnroqueLargo { get; set; }
    public bool NegroEnroqueCorto { get; set; }
    public bool NegroEnroqueLargo { get; set; }
    public int? CasillaAlPasoFila { get; set; }
    public int? CasillaAlPasoColumna { get; set; }
    public int ContadorMediosMovimientosSinCaptura { get; set; }
    public string? Ganador { get; set; }
    public string? MotivoFinalizacion { get; set; }

    public List<PiezaTableroEntity> Piezas { get; set; } = [];
    public List<MovimientoEntity> Movimientos { get; set; } = [];
    public List<ClavePosicionEntity> ClavesPosicion { get; set; } = [];
}
