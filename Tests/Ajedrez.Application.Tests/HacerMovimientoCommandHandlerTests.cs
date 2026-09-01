using Ajedrez.Application.Excepciones;
using Ajedrez.Application.Partidas.Commands.HacerMovimiento;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Excepciones;
using Ajedrez.Domain.Servicios;
using Xunit;

namespace Ajedrez.Application.Tests;

public class HacerMovimientoCommandHandlerTests
{
    private static (HacerMovimientoCommandHandler Handler, Partida Partida) CrearEscenario()
    {
        var repositorio = new PartidaRepositoryFalso();
        var partida = Partida.Iniciar();
        repositorio.GuardarAsync(partida, CancellationToken.None).GetAwaiter().GetResult();

        var handler = new HacerMovimientoCommandHandler(
            repositorio,
            new ValidadorMovimientos(),
            new DetectorFinDePartida(),
            new DespachadorEventosDominioFalso());

        return (handler, partida);
    }

    [Fact]
    public async Task Handle_movimientoLegal_actualizaElTableroYCambiaElTurno()
    {
        var (handler, partida) = CrearEscenario();

        var dto = await handler.Handle(new HacerMovimientoCommand(partida.Id, "e2", "e4"), CancellationToken.None);

        Assert.Equal("Negro", dto.TurnoActual);
        Assert.Single(dto.Historial);
        Assert.Equal("e2", dto.Historial[0].Origen);
        Assert.Equal("e4", dto.Historial[0].Destino);
    }

    [Fact]
    public async Task Handle_movimientoIlegal_lanzaExcepcionDeDominio()
    {
        var (handler, partida) = CrearEscenario();

        await Assert.ThrowsAsync<MovimientoIlegalException>(() =>
            handler.Handle(new HacerMovimientoCommand(partida.Id, "e2", "e5"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_partidaInexistente_lanzaPartidaNoEncontrada()
    {
        var repositorio = new PartidaRepositoryFalso();
        var handler = new HacerMovimientoCommandHandler(
            repositorio, new ValidadorMovimientos(), new DetectorFinDePartida(), new DespachadorEventosDominioFalso());

        await Assert.ThrowsAsync<PartidaNoEncontradaException>(() =>
            handler.Handle(new HacerMovimientoCommand(Guid.NewGuid(), "e2", "e4"), CancellationToken.None));
    }
}
