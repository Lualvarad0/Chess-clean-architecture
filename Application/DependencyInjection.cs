using Ajedrez.Application.Comportamientos;
using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Servicios;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ajedrez.Application;

public static class DependencyInjection
{
    public static IServiceCollection AgregarApplication(this IServiceCollection servicios)
    {
        servicios.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        servicios.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        servicios.AddTransient(typeof(IPipelineBehavior<,>), typeof(ComportamientoValidacion<,>));

        servicios.AddScoped<IDespachadorEventosDominio, Eventos.DespachadorEventosDominio>();
        servicios.AddScoped<IValidadorMovimientos, ValidadorMovimientos>();
        servicios.AddScoped<IDetectorFinDePartida, DetectorFinDePartida>();

        return servicios;
    }
}
