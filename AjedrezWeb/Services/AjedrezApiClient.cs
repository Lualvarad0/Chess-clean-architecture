using System.Net.Http.Json;
using Ajedrez.Shared.Dtos;

namespace AjedrezWeb.Services;

public sealed record ResultadoLlamadaApi<T>(bool Exitoso, T? Datos, string? MensajeError);

public sealed class AjedrezApiClient(HttpClient http)
{
    public async Task<PartidaDto> IniciarPartidaAsync(CancellationToken cancellationToken = default)
    {
        var respuesta = await http.PostAsync("api/partidas", null, cancellationToken);
        respuesta.EnsureSuccessStatusCode();
        return (await respuesta.Content.ReadFromJsonAsync<PartidaDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<MovimientosLegalesDto> ObtenerMovimientosLegalesAsync(Guid partidaId, string casilla, CancellationToken cancellationToken = default)
    {
        var respuesta = await http.GetAsync($"api/partidas/{partidaId}/movimientos-legales?casilla={casilla}", cancellationToken);
        respuesta.EnsureSuccessStatusCode();
        return (await respuesta.Content.ReadFromJsonAsync<MovimientosLegalesDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<ResultadoLlamadaApi<PartidaDto>> HacerMovimientoAsync(Guid partidaId, HacerMovimientoRequestDto request, CancellationToken cancellationToken = default)
    {
        var respuesta = await http.PostAsJsonAsync($"api/partidas/{partidaId}/movimientos", request, cancellationToken);
        return await LeerResultadoAsync<PartidaDto>(respuesta, cancellationToken);
    }

    public async Task<ResultadoLlamadaApi<PartidaDto>> ObtenerJugadaIAAsync(Guid partidaId, string nivel, CancellationToken cancellationToken = default)
    {
        var respuesta = await http.PostAsync($"api/partidas/{partidaId}/jugada-ia?nivel={nivel}", null, cancellationToken);
        return await LeerResultadoAsync<PartidaDto>(respuesta, cancellationToken);
    }

    private static async Task<ResultadoLlamadaApi<T>> LeerResultadoAsync<T>(HttpResponseMessage respuesta, CancellationToken cancellationToken)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            var datos = await respuesta.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            return new ResultadoLlamadaApi<T>(true, datos, null);
        }

        var problema = await respuesta.Content.ReadFromJsonAsync<ProblemaApi>(cancellationToken: cancellationToken);
        return new ResultadoLlamadaApi<T>(false, default, problema?.Detail ?? problema?.Title ?? "Ocurrió un error inesperado.");
    }

    private sealed record ProblemaApi(string? Title, string? Detail);
}
