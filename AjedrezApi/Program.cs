using Ajedrez.Application;
using Ajedrez.Infrastructure;
using Ajedrez.Infrastructure.IA;
using Ajedrez.Infrastructure.Persistencia;
using AjedrezApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AgregarApplication();
builder.Services.AgregarInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<ExcepcionesDominioHandler>();
builder.Services.AddProblemDetails();

// CORS para que AjedrezWeb (Blazor WASM, otro origen) pueda llamar a esta API.
const string PoliticaCorsBlazor = "AjedrezWeb";
builder.Services.AddCors(opciones => opciones.AddPolicy(PoliticaCorsBlazor, politica => politica
    .WithOrigins(builder.Configuration.GetSection("OrigenesPermitidos").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Sin migraciones todavía: para el ejercicio basta con crear el esquema si no existe.
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AjedrezDbContext>();
    contexto.Database.EnsureCreated();

    if (!contexto.EntradasLibroApertura.Any())
    {
        contexto.EntradasLibroApertura.AddRange(SembradorLibroApertura.GenerarEntradas());
        contexto.SaveChanges();
    }
}

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Sin UseHttpsRedirection a propósito: el ejercicio corre todo por HTTP entre AjedrezApi y
// AjedrezWeb para no tener que lidiar con el certificado de desarrollo; redirigir a https
// rompía el fetch de Blazor porque cambiaba el origen a mitad de la petición.
app.UseCors(PoliticaCorsBlazor);

app.UseAuthorization();

app.MapControllers();

app.Run();
