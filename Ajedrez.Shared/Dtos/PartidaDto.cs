namespace Ajedrez.Shared.Dtos;

public sealed record CasillaDto(string Posicion, string TipoPieza, string Color);

public sealed record TableroDto(IReadOnlyList<CasillaDto> Piezas);

public sealed record MovimientoDto(string Origen, string Destino, string PiezaMovida, string? PromocionA = null);

public sealed record ResultadoPartidaDto(string? Ganador, string Motivo);

public sealed record PartidaDto(
    Guid Id,
    TableroDto Tablero,
    string TurnoActual,
    string Estado,
    IReadOnlyList<MovimientoDto> Historial,
    ResultadoPartidaDto? Resultado);
