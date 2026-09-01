using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.IniciarPartida;

public sealed record IniciarPartidaCommand : IRequest<PartidaDto>;
