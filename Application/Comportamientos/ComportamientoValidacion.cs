using FluentValidation;
using MediatR;

namespace Ajedrez.Application.Comportamientos;

public sealed class ComportamientoValidacion<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validadores)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validadores.Any())
            return await next(cancellationToken);

        var contexto = new ValidationContext<TRequest>(request);
        var resultados = await Task.WhenAll(validadores.Select(v => v.ValidateAsync(contexto, cancellationToken)));
        var errores = resultados.SelectMany(r => r.Errors).ToList();

        if (errores.Count != 0)
            throw new ValidationException(errores);

        return await next(cancellationToken);
    }
}
