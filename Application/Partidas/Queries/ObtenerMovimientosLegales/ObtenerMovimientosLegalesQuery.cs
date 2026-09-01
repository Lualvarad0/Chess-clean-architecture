using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Queries.ObtenerMovimientosLegales;

public sealed record ObtenerMovimientosLegalesQuery(Guid PartidaId, string Casilla) : IRequest<MovimientosLegalesDto>;
