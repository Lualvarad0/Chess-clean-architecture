using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.Servicios;
using Ajedrez.Domain.ValueObjects;
using Ajedrez.Infrastructure.IA;
using Ajedrez.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ajedrez.Infrastructure.Tests;

public class MotorIAHibridoTests : IDisposable
{
    private readonly AjedrezDbContext _contexto;
    private readonly MotorIAHibrido _motor;

    public MotorIAHibridoTests()
    {
        var nombreBaseDatos = $"AjedrezTests_{Guid.NewGuid():N}";
        var cadenaConexion = $@"Server=(localdb)\MSSQLLocalDB;Database={nombreBaseDatos};Trusted_Connection=True;TrustServerCertificate=True";
        var opciones = new DbContextOptionsBuilder<AjedrezDbContext>().UseSqlServer(cadenaConexion).Options;

        _contexto = new AjedrezDbContext(opciones);
        _contexto.Database.EnsureCreated();

        _motor = new MotorIAHibrido(_contexto);
    }

    public void Dispose()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    private static string ClaveInicial()
    {
        var detectorFin = new DetectorFinDePartida();
        var contexto = new ContextoMovimiento(null, DerechosEnroque.Iniciales());
        return detectorFin.ClavePosicion(Tablero.PosicionInicial(), Color.Blanco, contexto);
    }

    [Fact]
    public async Task ObtenerMejorMovimiento_conUnaSolaEntradaDeLibro_laDevuelveSinImportarElNivel()
    {
        _contexto.EntradasLibroApertura.Add(new EntradaLibroAperturaEntity { ClavePosicion = ClaveInicial(), Origen = "e2", Destino = "e4" });
        await _contexto.SaveChangesAsync();

        var partida = Partida.Iniciar();
        var movimiento = await _motor.ObtenerMejorMovimientoAsync(partida, NivelDificultad.Profesional, CancellationToken.None);

        Assert.Equal("e2", movimiento.Origen.ANotacionAlgebraica());
        Assert.Equal("e4", movimiento.Destino.ANotacionAlgebraica());
    }

    [Fact]
    public async Task ObtenerMejorMovimiento_profesional_siempreEligeLaEntradaConMejorPuntaje()
    {
        var clave = ClaveInicial();
        _contexto.EntradasLibroApertura.AddRange(
            new EntradaLibroAperturaEntity { ClavePosicion = clave, Origen = "e2", Destino = "e4", Victorias = 1, Derrotas = 5 },
            new EntradaLibroAperturaEntity { ClavePosicion = clave, Origen = "d2", Destino = "d4", Victorias = 10, Derrotas = 0 });
        await _contexto.SaveChangesAsync();

        var partida = Partida.Iniciar();

        for (int i = 0; i < 10; i++)
        {
            var movimiento = await _motor.ObtenerMejorMovimientoAsync(partida, NivelDificultad.Profesional, CancellationToken.None);
            Assert.Equal("d2", movimiento.Origen.ANotacionAlgebraica());
            Assert.Equal("d4", movimiento.Destino.ANotacionAlgebraica());
        }
    }

    [Fact]
    public async Task ObtenerMejorMovimiento_facil_varíaEntreLasOpcionesDelLibro()
    {
        var clave = ClaveInicial();
        _contexto.EntradasLibroApertura.AddRange(
            new EntradaLibroAperturaEntity { ClavePosicion = clave, Origen = "e2", Destino = "e4" },
            new EntradaLibroAperturaEntity { ClavePosicion = clave, Origen = "d2", Destino = "d4" });
        await _contexto.SaveChangesAsync();

        var partida = Partida.Iniciar();
        var destinosVistos = new HashSet<string>();

        for (int i = 0; i < 30; i++)
        {
            var movimiento = await _motor.ObtenerMejorMovimientoAsync(partida, NivelDificultad.Facil, CancellationToken.None);
            destinosVistos.Add(movimiento.Destino.ANotacionAlgebraica());
        }

        Assert.True(destinosVistos.Count > 1, "Con 30 tiradas al azar entre 2 opciones, se esperaba ver ambas.");
    }

    [Fact]
    public async Task ObtenerMejorMovimiento_sinEntradaDeLibro_caeAMinimaxYDevuelveUnMovimientoLegal()
    {
        // Libro vacío: cualquier posición fuerza el fallback a minimax.
        var partida = Partida.Iniciar();

        var movimiento = await _motor.ObtenerMejorMovimientoAsync(partida, NivelDificultad.Facil, CancellationToken.None);

        var validador = new ValidadorMovimientos();
        var contexto = new ContextoMovimiento(partida.CasillaAlPaso, partida.DerechosEnroque);
        var legales = validador.MovimientosLegales(partida.Tablero, movimiento.Origen, partida.TurnoActual, contexto);

        Assert.Contains(legales, m => m.Destino == movimiento.Destino);
    }
}
