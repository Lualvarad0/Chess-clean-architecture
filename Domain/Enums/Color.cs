namespace Ajedrez.Domain.Enums;

public enum Color
{
    Blanco,
    Negro
}

public static class ColorExtensions
{
    public static Color Opuesto(this Color color) => color == Color.Blanco ? Color.Negro : Color.Blanco;
}
