using System.Text.RegularExpressions;
using Ajedrez.Domain.Enums;
using FluentValidation;

namespace Ajedrez.Application.Partidas.Commands.HacerMovimiento;

public sealed partial class HacerMovimientoCommandValidator : AbstractValidator<HacerMovimientoCommand>
{
    public HacerMovimientoCommandValidator()
    {
        RuleFor(c => c.PartidaId).NotEmpty();

        RuleFor(c => c.Origen).Matches(PatronCasilla()).WithMessage("El origen debe ser una casilla válida, ej. 'e2'.");
        RuleFor(c => c.Destino).Matches(PatronCasilla()).WithMessage("El destino debe ser una casilla válida, ej. 'e4'.");

        RuleFor(c => c.PromocionA)
            .Must(valor => valor is null || Enum.TryParse<TipoPieza>(valor, ignoreCase: true, out _))
            .WithMessage("PromocionA debe ser Dama, Torre, Alfil o Caballo.");
    }

    [GeneratedRegex("^[a-h][1-8]$")]
    private static partial Regex PatronCasilla();
}
