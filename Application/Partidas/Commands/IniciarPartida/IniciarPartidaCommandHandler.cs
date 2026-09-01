using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Entidades;
using Ajedrez.Shared.Dtos;
using MediatR;

namespace Ajedrez.Application.Partidas.Commands.IniciarPartida;

public sealed class IniciarPartidaCommandHandler(IPartidaRepository repositorio, IDespachadorEventosDominio despachador)
    : IRequestHandler<IniciarPartidaCommand, PartidaDto>
{
    public async Task<PartidaDto> Handle(IniciarPartidaCommand request, CancellationToken cancellationToken)
    {
        var partida = Partida.Iniciar();

        await repositorio.GuardarAsync(partida, cancellationToken);
        await despachador.DespacharYLimpiarAsync(partida, cancellationToken);

        return MapeadorPartida.ADto(partida);
    }
}
