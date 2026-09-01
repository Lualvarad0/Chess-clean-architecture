using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.Especificaciones;

public sealed class ProveedorEspecificaciones
{
    private readonly IReadOnlyDictionary<TipoPieza, IEspecificacionMovimientoPieza> _especificaciones;

    public ProveedorEspecificaciones()
    {
        IEspecificacionMovimientoPieza[] todas =
        [
            new EspecificacionMovimientoPeon(),
            new EspecificacionMovimientoCaballo(),
            new EspecificacionMovimientoAlfil(),
            new EspecificacionMovimientoTorre(),
            new EspecificacionMovimientoDama(),
            new EspecificacionMovimientoRey()
        ];

        _especificaciones = todas.ToDictionary(e => e.Tipo);
    }

    public IEspecificacionMovimientoPieza Para(TipoPieza tipo) => _especificaciones[tipo];
}
