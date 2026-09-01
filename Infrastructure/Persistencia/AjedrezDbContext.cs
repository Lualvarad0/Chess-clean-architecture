using Microsoft.EntityFrameworkCore;

namespace Ajedrez.Infrastructure.Persistencia;

public sealed class AjedrezDbContext(DbContextOptions<AjedrezDbContext> options) : DbContext(options)
{
    public DbSet<PartidaEntity> Partidas => Set<PartidaEntity>();
    public DbSet<PiezaTableroEntity> PiezasTablero => Set<PiezaTableroEntity>();
    public DbSet<MovimientoEntity> Movimientos => Set<MovimientoEntity>();
    public DbSet<ClavePosicionEntity> ClavesPosicion => Set<ClavePosicionEntity>();
    public DbSet<EntradaLibroAperturaEntity> EntradasLibroApertura => Set<EntradaLibroAperturaEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartidaEntity>(entidad =>
        {
            entidad.ToTable("Partidas");
            entidad.HasKey(p => p.Id);

            entidad.HasMany(p => p.Piezas)
                .WithOne()
                .HasForeignKey(p => p.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            entidad.HasMany(p => p.Movimientos)
                .WithOne()
                .HasForeignKey(m => m.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);

            entidad.HasMany(p => p.ClavesPosicion)
                .WithOne()
                .HasForeignKey(c => c.PartidaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PiezaTableroEntity>(entidad =>
        {
            entidad.ToTable("PiezasTablero");
            entidad.HasKey(p => p.Id);
            entidad.Property(p => p.Tipo).IsRequired().HasMaxLength(20);
            entidad.Property(p => p.Color).IsRequired().HasMaxLength(10);
            entidad.HasIndex(p => p.PartidaId);
        });

        modelBuilder.Entity<MovimientoEntity>(entidad =>
        {
            entidad.ToTable("Movimientos");
            entidad.HasKey(m => m.Id);
            entidad.Property(m => m.PromocionA).HasMaxLength(20);
            entidad.HasIndex(m => new { m.PartidaId, m.Orden });
        });

        modelBuilder.Entity<ClavePosicionEntity>(entidad =>
        {
            entidad.ToTable("ClavesPosicion");
            entidad.HasKey(c => c.Id);
            // Sin índice sobre Clave: es un string largo (codifica las 64 casillas + contexto),
            // pasa el límite de 900 bytes para clave de índice en SQL Server. La tabla es chica
            // (una fila por jugada de una partida), así que no hace falta.
            entidad.Property(c => c.Clave).IsRequired();
            entidad.HasIndex(c => new { c.PartidaId, c.Orden });
        });

        modelBuilder.Entity<EntradaLibroAperturaEntity>(entidad =>
        {
            entidad.ToTable("EntradasLibroApertura");
            entidad.HasKey(e => e.Id);
            entidad.Property(e => e.ClavePosicion).IsRequired();
            entidad.Property(e => e.Origen).IsRequired().HasMaxLength(2);
            entidad.Property(e => e.Destino).IsRequired().HasMaxLength(2);
        });
    }
}
