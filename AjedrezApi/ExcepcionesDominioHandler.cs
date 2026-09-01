using Ajedrez.Application.Excepciones;
using Ajedrez.Domain.Excepciones;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AjedrezApi;

// Los controllers no atrapan excepciones (se mantienen finos, solo orquestan MediatR);
// este handler central traduce las excepciones de Application/Domain a respuestas HTTP.
public sealed class ExcepcionesDominioHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, titulo, detalle) = exception switch
        {
            PartidaNoEncontradaException e => (StatusCodes.Status404NotFound, "Partida no encontrada", e.Message),
            MovimientoIlegalException e => (StatusCodes.Status400BadRequest, "Movimiento ilegal", e.Message),
            ValidationException e => (StatusCodes.Status400BadRequest, "Solicitud inválida", string.Join("; ", e.Errors.Select(err => err.ErrorMessage))),
            FormatException e => (StatusCodes.Status400BadRequest, "Solicitud inválida", e.Message),
            ArgumentOutOfRangeException e => (StatusCodes.Status400BadRequest, "Solicitud inválida", e.Message),
            _ => (0, string.Empty, string.Empty)
        };

        if (statusCode == 0)
            return false;

        var problema = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = detalle
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);
        return true;
    }
}
