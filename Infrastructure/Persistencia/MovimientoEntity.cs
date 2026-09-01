namespace Ajedrez.Infrastructure.Persistencia;

// Una fila por cada movimiento jugado, en orden. Reemplaza al arreglo de historial que antes
// vivía embebido en un JSON.
public sealed class MovimientoEntity
{
    public int Id { get; set; }
    public Guid PartidaId { get; set; }
    public int Orden { get; set; }
    public int OrigenFila { get; set; }
    public int OrigenColumna { get; set; }
    public int DestinoFila { get; set; }
    public int DestinoColumna { get; set; }
    public string? PromocionA { get; set; }
    public bool EsEnroqueCorto { get; set; }
    public bool EsEnroqueLargo { get; set; }
    public bool EsCapturaAlPaso { get; set; }
}
