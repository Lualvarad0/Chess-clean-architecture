using Ajedrez.Application.Partidas.Commands.HacerMovimiento;
using Ajedrez.Application.Partidas.Commands.IniciarPartida;
using Ajedrez.Application.Partidas.Commands.ObtenerJugadaIA;
using Ajedrez.Application.Partidas.Queries.ObtenerEstadoPartida;
using Ajedrez.Application.Partidas.Queries.ObtenerMovimientosLegales;
using Ajedrez.Domain.Enums;
using Ajedrez.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AjedrezApi.Controllers;

[ApiController]
[Route("api/partidas")]
public sealed class PartidasController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PartidaDto>> IniciarPartida(CancellationToken cancellationToken)
    {
        var partida = await mediator.Send(new IniciarPartidaCommand(), cancellationToken);
        return CreatedAtAction(nameof(ObtenerPartida), new { id = partida.Id }, partida);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PartidaDto>> ObtenerPartida(Guid id, CancellationToken cancellationToken)
    {
        var partida = await mediator.Send(new ObtenerEstadoPartidaQuery(id), cancellationToken);
        return Ok(partida);
    }

    [HttpPost("{id:guid}/movimientos")]
    public async Task<ActionResult<PartidaDto>> HacerMovimiento(
        Guid id,
        [FromBody] HacerMovimientoRequestDto request,
        CancellationToken cancellationToken)
    {
        var comando = new HacerMovimientoCommand(id, request.Origen, request.Destino, request.PromocionA);
        var partida = await mediator.Send(comando, cancellationToken);
        return Ok(partida);
    }

    [HttpGet("{id:guid}/movimientos-legales")]
    public async Task<ActionResult<MovimientosLegalesDto>> ObtenerMovimientosLegales(
        Guid id,
        [FromQuery] string casilla,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(new ObtenerMovimientosLegalesQuery(id, casilla), cancellationToken);
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/jugada-ia")]
    public async Task<ActionResult<PartidaDto>> ObtenerJugadaIA(
        Guid id,
        [FromQuery] NivelDificultad nivel,
        CancellationToken cancellationToken)
    {
        var partida = await mediator.Send(new ObtenerJugadaIACommand(id, nivel), cancellationToken);
        return Ok(partida);
    }
}
