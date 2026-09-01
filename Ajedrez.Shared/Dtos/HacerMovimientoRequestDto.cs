namespace Ajedrez.Shared.Dtos;

public sealed record HacerMovimientoRequestDto(string Origen, string Destino, string? PromocionA = null);
