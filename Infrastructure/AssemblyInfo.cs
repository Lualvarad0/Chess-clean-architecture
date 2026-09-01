using System.Runtime.CompilerServices;

// Permite que las pruebas accedan a los tipos internal del motor de IA (EvaluadorTablero,
// BuscadorMinimax) sin tener que hacerlos públicos solo para poder probarlos.
[assembly: InternalsVisibleTo("Ajedrez.Infrastructure.Tests")]
