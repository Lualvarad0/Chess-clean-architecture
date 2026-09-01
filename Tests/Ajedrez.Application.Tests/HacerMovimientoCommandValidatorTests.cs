using Ajedrez.Application.Partidas.Commands.HacerMovimiento;
using Xunit;

namespace Ajedrez.Application.Tests;

public class HacerMovimientoCommandValidatorTests
{
    private readonly HacerMovimientoCommandValidator _validator = new();

    [Fact]
    public void Comando_valido_pasaLaValidacion()
    {
        var resultado = _validator.Validate(new HacerMovimientoCommand(Guid.NewGuid(), "e2", "e4"));

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData("z9")]
    [InlineData("e0")]
    [InlineData("")]
    public void Origen_invalido_fallaLaValidacion(string origenInvalido)
    {
        var resultado = _validator.Validate(new HacerMovimientoCommand(Guid.NewGuid(), origenInvalido, "e4"));

        Assert.False(resultado.IsValid);
    }
}
