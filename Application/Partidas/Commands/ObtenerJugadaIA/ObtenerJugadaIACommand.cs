using Ajedrez.Domain.Enums;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.ObtenerJugadaIA;

public sealed record ObtenerJugadaIACommand(Guid PartidaId, NivelDificultad Nivel) : IRequest<PartidaDto>;
