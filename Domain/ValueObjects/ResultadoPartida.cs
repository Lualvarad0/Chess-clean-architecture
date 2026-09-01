using Ajedrez.Domain.Enums;

namespace Ajedrez.Domain.ValueObjects;

public readonly record struct ResultadoPartida(Color? Ganador, MotivoFinalizacion Motivo);
