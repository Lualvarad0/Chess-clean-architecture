using FluentValidation;

namespace Ajedrez.Application.Partidas.Commands.ObtenerJugadaIA;

public sealed class ObtenerJugadaIACommandValidator : AbstractValidator<ObtenerJugadaIACommand>
{
    public ObtenerJugadaIACommandValidator()
    {
        RuleFor(c => c.PartidaId).NotEmpty();
        RuleFor(c => c.Nivel).IsInEnum();
    }
}
