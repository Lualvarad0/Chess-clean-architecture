namespace Ajedrez.Domain.ValueObjects;

public readonly record struct Posicion
{
    public int Fila { get; }
    public int Columna { get; }

    private Posicion(int fila, int columna)
    {
        Fila = fila;
        Columna = columna;
    }

    public static bool EnRango(int fila, int columna) => fila is >= 0 and <= 7 && columna is >= 0 and <= 7;

    public static Posicion Crear(int fila, int columna)
    {
        if (!EnRango(fila, columna))
            throw new ArgumentOutOfRangeException(nameof(fila), $"La posición ({fila},{columna}) está fuera del tablero.");

        return new Posicion(fila, columna);
    }

    public static bool TryDesplazar(Posicion origen, int deltaFila, int deltaColumna, out Posicion resultado)
    {
        int fila = origen.Fila + deltaFila;
        int columna = origen.Columna + deltaColumna;

        if (EnRango(fila, columna))
        {
            resultado = new Posicion(fila, columna);
            return true;
        }

        resultado = default;
        return false;
    }

    public static Posicion DesdeNotacionAlgebraica(string notacion)
    {
        if (notacion.Length != 2)
            throw new FormatException($"Notación algebraica inválida: '{notacion}'.");

        int columna = char.ToLowerInvariant(notacion[0]) - 'a';
        int fila = notacion[1] - '1';
        return Crear(fila, columna);
    }

    public string ANotacionAlgebraica() => $"{(char)('a' + Columna)}{Fila + 1}";

    public override string ToString() => ANotacionAlgebraica();
}
