using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.HacerMovimiento;

public sealed record HacerMovimientoCommand(Guid PartidaId, string Origen, string Destino, string? PromocionA = null)
    : IRequest<PartidaDto>;
