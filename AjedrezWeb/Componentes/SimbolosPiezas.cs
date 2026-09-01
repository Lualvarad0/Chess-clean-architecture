namespace AjedrezWeb.Componentes;

internal static class SimbolosPiezas
{
    public static string De(string tipo, string color) => (tipo, color) switch
    {
        ("Rey", "Blanco") => "♔",
        ("Dama", "Blanco") => "♕",
        ("Torre", "Blanco") => "♖",
        ("Alfil", "Blanco") => "♗",
        ("Caballo", "Blanco") => "♘",
        ("Peon", "Blanco") => "♙",
        ("Rey", "Negro") => "♚",
        ("Dama", "Negro") => "♛",
        ("Torre", "Negro") => "♜",
        ("Alfil", "Negro") => "♝",
        ("Caballo", "Negro") => "♞",
        ("Peon", "Negro") => "♟",
        _ => "?"
    };
}
