using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ajedrez.Infrastructure.Tests;

public class PartidaRepositoryTests : IDisposable
{
    private readonly AjedrezDbContext _contexto;
    private readonly PartidaRepository _repositorio;

    public PartidaRepositoryTests()
    {
        // Base de datos real en LocalDB, con nombre único por ejecución de test para no
        // pisar otras corridas en paralelo; se borra en Dispose().
        var nombreBaseDatos = $"AjedrezTests_{Guid.NewGuid():N}";
        var cadenaConexion = $@"Server=(localdb)\MSSQLLocalDB;Database={nombreBaseDatos};Trusted_Connection=True;TrustServerCertificate=True";

        var opciones = new DbContextOptionsBuilder<AjedrezDbContext>().UseSqlServer(cadenaConexion).Options;

        _contexto = new AjedrezDbContext(opciones);
        _contexto.Database.EnsureCreated();

        _repositorio = new PartidaRepository(_contexto);
    }

    public void Dispose()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    private static Movimiento Mov(string origen, string destino) =>
        new(Posicion.DesdeNotacionAlgebraica(origen), Posicion.DesdeNotacionAlgebraica(destino));

    [Fact]
    public async Task GuardarYObtener_partidaRecienIniciada_conservaElEstado()
    {
        var partida = Partida.Iniciar();

        await _repositorio.GuardarAsync(partida, CancellationToken.None);
        var recuperada = await _repositorio.ObtenerPorIdAsync(partida.Id, CancellationToken.None);

        Assert.NotNull(recuperada);
        Assert.Equal(partida.Id, recuperada!.Id);
        Assert.Equal(Color.Blanco, recuperada.TurnoActual);
        Assert.Equal(EstadoPartida.EnCurso, recuperada.Estado);

        int totalPiezas = recuperada.Tablero.PiezasDeColor(Color.Blanco).Count() + recuperada.Tablero.PiezasDeColor(Color.Negro).Count();
        Assert.Equal(32, totalPiezas);
    }

    [Fact]
    public async Task GuardarYObtener_despuesDeMovimientos_conservaTableroYHistorialYSePuedeSeguirJugando()
    {
        var validador = new ValidadorMovimientos();
        var detectorFin = new DetectorFinDePartida();
        var partida = Partida.Iniciar();

        partida.RealizarMovimiento(Mov("e2", "e4"), validador, detectorFin);
        partida.RealizarMovimiento(Mov("a7", "a6"), validador, detectorFin);
        await _repositorio.GuardarAsync(partida, CancellationToken.None);

        var recuperada = await _repositorio.ObtenerPorIdAsync(partida.Id, CancellationToken.None);

        Assert.NotNull(recuperada);
        Assert.Equal(2, recuperada!.Historial.Count);
        Assert.Equal(Color.Blanco, recuperada.TurnoActual);

        var piezaEnE4 = recuperada.Tablero.ObtenerPiezaEn(Posicion.DesdeNotacionAlgebraica("e4"));
        Assert.NotNull(piezaEnE4);
        Assert.Equal(new Pieza(TipoPieza.Peon, Color.Blanco), piezaEnE4!.Value);

        // Sobre la partida recuperada se puede seguir jugando con normalidad.
        recuperada.RealizarMovimiento(Mov("e4", "e5"), validador, detectorFin);
        Assert.Equal(Color.Negro, recuperada.TurnoActual);
    }

    [Fact]
    public async Task ObtenerPorId_partidaInexistente_devuelveNull()
    {
        var recuperada = await _repositorio.ObtenerPorIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(recuperada);
    }

    [Fact]
    public async Task Guardar_dosVeces_actualizaEnLugarDeDuplicar()
    {
        var validador = new ValidadorMovimientos();
        var detectorFin = new DetectorFinDePartida();
        var partida = Partida.Iniciar();

        await _repositorio.GuardarAsync(partida, CancellationToken.None);

        partida.RealizarMovimiento(Mov("g1", "f3"), validador, detectorFin);
        await _repositorio.GuardarAsync(partida, CancellationToken.None);

        var recuperada = await _repositorio.ObtenerPorIdAsync(partida.Id, CancellationToken.None);

        Assert.NotNull(recuperada);
        Assert.Single(recuperada!.Historial);
        Assert.Equal(1, await _contexto.Partidas.CountAsync());
    }
}
