using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Queries.ObtenerEstadoPartida;

public sealed record ObtenerEstadoPartidaQuery(Guid PartidaId) : IRequest<PartidaDto>;
