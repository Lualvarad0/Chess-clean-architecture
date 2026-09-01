using Ajedrez.Domain.Entidades;
using Ajedrez.Domain.Enums;
using Ajedrez.Domain.ValueObjects;

namespace Ajedrez.Infrastructure.Persistencia;

public static class PartidaEntityMapper
{
    public static PartidaEntity AEntity(Partida partida)
    {
        var piezas = new List<PiezaTableroEntity>();

        foreach (var color in new[] { Color.Blanco, Color.Negro })
        {
            foreach (var (posicion, pieza) in partida.Tablero.PiezasDeColor(color))
            {
                piezas.Add(new PiezaTableroEntity
                {
                    PartidaId = partida.Id,
                    Fila = posicion.Fila,
                    Columna = posicion.Columna,
                    Tipo = pieza.Tipo.ToString(),
                    Color = pieza.Color.ToString()
                });
            }
        }

        var movimientos = partida.Historial.Select((m, indice) => new MovimientoEntity
        {
            PartidaId = partida.Id,
            Orden = indice,
            OrigenFila = m.Origen.Fila,
            OrigenColumna = m.Origen.Columna,
            DestinoFila = m.Destino.Fila,
            DestinoColumna = m.Destino.Columna,
            PromocionA = m.PromocionA?.ToString(),
            EsEnroqueCorto = m.EsEnroqueCorto,
            EsEnroqueLargo = m.EsEnroqueLargo,
            EsCapturaAlPaso = m.EsCapturaAlPaso
        }).ToList();

        var clavesPosicion = partida.HistorialClavesPosicion.Select((clave, indice) => new ClavePosicionEntity
        {
            PartidaId = partida.Id,
            Orden = indice,
            Clave = clave
        }).ToList();

        return new PartidaEntity
        {
            Id = partida.Id,
            TurnoActual = partida.TurnoActual.ToString(),
            Estado = partida.Estado.ToString(),
            BlancoEnroqueCorto = partida.DerechosEnroque.BlancoEnroqueCorto,
            BlancoEnroqueLargo = partida.DerechosEnroque.BlancoEnroqueLargo,
            NegroEnroqueCorto = partida.DerechosEnroque.NegroEnroqueCorto,
            NegroEnroqueLargo = partida.DerechosEnroque.NegroEnroqueLargo,
            CasillaAlPasoFila = partida.CasillaAlPaso?.Fila,
            CasillaAlPasoColumna = partida.CasillaAlPaso?.Columna,
            ContadorMediosMovimientosSinCaptura = partida.ContadorMediosMovimientosSinCaptura,
            Ganador = partida.Resultado?.Ganador?.ToString(),
            MotivoFinalizacion = partida.Resultado?.Motivo.ToString(),
            Piezas = piezas,
            Movimientos = movimientos,
            ClavesPosicion = clavesPosicion
        };
    }

    public static Partida ADominio(PartidaEntity entidad)
    {
        var tablero = Tablero.Vacio();
        foreach (var p in entidad.Piezas)
        {
            var posicion = Posicion.Crear(p.Fila, p.Columna);
            var pieza = new Pieza(Enum.Parse<TipoPieza>(p.Tipo), Enum.Parse<Color>(p.Color));
            tablero = tablero.ConPieza(posicion, pieza);
        }

        var derechosEnroque = new DerechosEnroque(
            entidad.BlancoEnroqueCorto, entidad.BlancoEnroqueLargo,
            entidad.NegroEnroqueCorto, entidad.NegroEnroqueLargo);

        Posicion? casillaAlPaso = entidad.CasillaAlPasoFila is int fila && entidad.CasillaAlPasoColumna is int columna
            ? Posicion.Crear(fila, columna)
            : null;

        ResultadoPartida? resultado = entidad.MotivoFinalizacion is null
            ? null
            : new ResultadoPartida(
                entidad.Ganador is null ? null : Enum.Parse<Color>(entidad.Ganador),
                Enum.Parse<MotivoFinalizacion>(entidad.MotivoFinalizacion));

        var historial = entidad.Movimientos
            .OrderBy(m => m.Orden)
            .Select(m => new Movimiento(
                Posicion.Crear(m.OrigenFila, m.OrigenColumna),
                Posicion.Crear(m.DestinoFila, m.DestinoColumna),
                m.PromocionA is null ? null : Enum.Parse<TipoPieza>(m.PromocionA),
                m.EsEnroqueCorto,
                m.EsEnroqueLargo,
                m.EsCapturaAlPaso))
            .ToList();

        var clavesPosicion = entidad.ClavesPosicion.OrderBy(c => c.Orden).Select(c => c.Clave).ToList();

        return Partida.Reconstruir(
            entidad.Id,
            tablero,
            Enum.Parse<Color>(entidad.TurnoActual),
            Enum.Parse<EstadoPartida>(entidad.Estado),
            derechosEnroque,
            casillaAlPaso,
            entidad.ContadorMediosMovimientosSinCaptura,
            resultado,
            historial,
            clavesPosicion);
    }
}
