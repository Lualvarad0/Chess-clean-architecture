using Ajedrez.Application.Eventos;
using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Eventos;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.IA;
using Ajedrez.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ajedrez.Infrastructure.Tests;

public class PartidaTerminadaActualizarLibroHandlerTests : IDisposable
{
    private readonly AjedrezDbContext _contexto;
    private readonly PartidaRepository _repositorioPartida;

    public PartidaTerminadaActualizarLibroHandlerTests()
    {
        var nombreBaseDatos = $"AjedrezTests_{Guid.NewGuid():N}";
        var cadenaConexion = $@"Server=(localdb)\MSSQLLocalDB;Database={nombreBaseDatos};Trusted_Connection=True;TrustServerCertificate=True";
        var opciones = new DbContextOptionsBuilder<AjedrezDbContext>().UseSqlServer(cadenaConexion).Options;

        _contexto = new AjedrezDbContext(opciones);
        _contexto.Database.EnsureCreated();

        _repositorioPartida = new PartidaRepository(_contexto);
    }

    public void Dispose()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [Fact]
    public async Task Handle_actualizaVictoriasDeLaEntradaDeLibroQueJugoElGanador()
    {
        var tableroInicial = Tablero.PosicionInicial();
        var contextoInicial = new ContextoMovimiento(null, DerechosEnroque.Iniciales());
        var claveInicial = new Ajedrez.Domain.Servicios.DetectorFinDePartida().ClavePosicion(tableroInicial, Color.Blanco, contextoInicial);

        var entrada = new EntradaLibroAperturaEntity { ClavePosicion = claveInicial, Origen = "e2", Destino = "e4" };
        _contexto.EntradasLibroApertura.Add(entrada);
        await _contexto.SaveChangesAsync();

        var movimiento = new Movimiento(Posicion.DesdeNotacionAlgebraica("e2"), Posicion.DesdeNotacionAlgebraica("e4"));
        var resultado = new ResultadoPartida(Color.Blanco, MotivoFinalizacion.JaqueMate);

        var partida = Partida.Reconstruir(
            Guid.NewGuid(),
            tableroInicial.AplicarMovimiento(movimiento),
            Color.Negro,
            EstadoPartida.Finalizada,
            DerechosEnroque.Iniciales(),
            null,
            0,
            resultado,
            [movimiento],
            []);

        await _repositorioPartida.GuardarAsync(partida, CancellationToken.None);

        var handler = new PartidaTerminadaActualizarLibroHandler(_contexto, _repositorioPartida);
        var notificacion = new DomainEventNotificacion<PartidaTerminadaEvent>(new PartidaTerminadaEvent(partida.Id, resultado));

        await handler.Handle(notificacion, CancellationToken.None);

        var entradaActualizada = await _contexto.EntradasLibroApertura.AsNoTracking().FirstAsync(e => e.Id == entrada.Id);
        Assert.Equal(1, entradaActualizada.VecesJugado);
        Assert.Equal(1, entradaActualizada.Victorias);
        Assert.Equal(0, entradaActualizada.Derrotas);
        Assert.Equal(0, entradaActualizada.Tablas);
    }
}
