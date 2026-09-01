using Ajedrez.Application.Partidas.Commands.IniciarPartida;
using Xunit;

namespace Ajedrez.Application.Tests;

public class IniciarPartidaCommandHandlerTests
{
    [Fact]
    public async Task Handle_creaPartidaConTableroInicialYTurnoBlanco()
    {
        var repositorio = new PartidaRepositoryFalso();
        var handler = new IniciarPartidaCommandHandler(repositorio, new DespachadorEventosDominioFalso());

        var dto = await handler.Handle(new IniciarPartidaCommand(), CancellationToken.None);

        Assert.Equal("Blanco", dto.TurnoActual);
        Assert.Equal("EnCurso", dto.Estado);
        Assert.Equal(32, dto.Tablero.Piezas.Count);
        Assert.NotEqual(Guid.Empty, dto.Id);

        var guardada = await repositorio.ObtenerPorIdAsync(dto.Id, CancellationToken.None);
        Assert.NotNull(guardada);
    }
}
