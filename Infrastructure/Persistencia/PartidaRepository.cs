using Ajedrez.Application.Interfaces;
using Ajedrez.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Ajedrez.Infrastructure.Persistencia;

public sealed class PartidaRepository(AjedrezDbContext contexto) : IPartidaRepository
{
    public async Task<Partida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entidad = await contexto.Partidas
            .Include(p => p.Piezas)
            .Include(p => p.Movimientos)
            .Include(p => p.ClavesPosicion)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entidad is null ? null : PartidaEntityMapper.ADominio(entidad);
    }

    public async Task GuardarAsync(Partida partida, CancellationToken cancellationToken)
    {
        var existente = await contexto.Partidas
            .Include(p => p.Piezas)
            .Include(p => p.Movimientos)
            .Include(p => p.ClavesPosicion)
            .FirstOrDefaultAsync(p => p.Id == partida.Id, cancellationToken);

        var nuevaEntidad = PartidaEntityMapper.AEntity(partida);

        if (existente is null)
        {
            contexto.Partidas.Add(nuevaEntidad);
        }
        else
        {
            // Las columnas escalares se actualizan campo a campo, pero el tablero, el historial
            // y las claves de repetición cambian de forma en cada jugada (piezas que se mueven o
            // desaparecen, un movimiento más) — se reemplazan por completo en vez de intentar
            // diffearlos jugada a jugada.
            contexto.Entry(existente).CurrentValues.SetValues(nuevaEntidad);

            contexto.PiezasTablero.RemoveRange(existente.Piezas);
            contexto.Movimientos.RemoveRange(existente.Movimientos);
            contexto.ClavesPosicion.RemoveRange(existente.ClavesPosicion);

            contexto.PiezasTablero.AddRange(nuevaEntidad.Piezas);
            contexto.Movimientos.AddRange(nuevaEntidad.Movimientos);
            contexto.ClavesPosicion.AddRange(nuevaEntidad.ClavesPosicion);
        }

        await contexto.SaveChangesAsync(cancellationToken);
    }
}
