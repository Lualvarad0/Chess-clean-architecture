using Ajedrez.Application.Partidas.Queries.ObtenerMovimientosLegales;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Servicios;
using Xunit;

namespace Ajedrez.Application.Tests;

public class ObtenerMovimientosLegalesQueryHandlerTests
{
    [Fact]
    public async Task Handle_peonEnE2_devuelveE3YE4()
    {
        var repositorio = new PartidaRepositoryFalso();
        var partida = Partida.Iniciar();
        await repositorio.GuardarAsync(partida, CancellationToken.None);

        var handler = new ObtenerMovimientosLegalesQueryHandler(repositorio, new ValidadorMovimientos());

        var dto = await handler.Handle(new ObtenerMovimientosLegalesQuery(partida.Id, "e2"), CancellationToken.None);

        Assert.Equal(2, dto.Destinos.Count);
        Assert.Contains("e3", dto.Destinos);
        Assert.Contains("e4", dto.Destinos);
    }
}
