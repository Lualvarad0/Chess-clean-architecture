using Ajedrez.Application.Interfaces;
using Ajedrez.Infrastructure.IA;
using Ajedrez.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ajedrez.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AgregarInfrastructure(this IServiceCollection servicios, IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("Ajedrez")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=AjedrezDb;Trusted_Connection=True;TrustServerCertificate=True";

        servicios.AddDbContext<AjedrezDbContext>(opciones => opciones.UseSqlServer(cadenaConexion));
        servicios.AddScoped<IPartidaRepository, PartidaRepository>();
        servicios.AddScoped<IMotorIA, MotorIAHibrido>();

        // Infrastructure tiene su propio event handler (PartidaTerminadaActualizarLibroHandler) que
        // necesita el DbContext, así que MediatR también escanea este ensamblado además del de Application.
        servicios.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return servicios;
    }
}
