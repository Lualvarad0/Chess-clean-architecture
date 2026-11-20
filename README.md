# Ajedrez — Clean Architecture

[![CI](https://github.com/Lualvarad0/Chess-clean-architecture/actions/workflows/ci.yml/badge.svg)](https://github.com/Lualvarad0/Chess-clean-architecture/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Ejercicio de aprendizaje: un juego de ajedrez completo (motor de reglas, IA rival y frontend web) construido en **.NET 10** siguiendo **Clean Architecture**, para practicar patrones tácticos de DDD que no se usan a diario (Specification, Domain Events, CQRS) fuera de un contexto laboral.

## Qué incluye

- **Motor de ajedrez completo**: todas las piezas y sus reglas de movimiento, jaque, jaque mate, ahogado, enroque, captura al paso, promoción, repetición triple y regla de los 50 movimientos.
- **Rival con IA híbrida**: combina un **libro de aperturas** (sembrado jugando 15 líneas conocidas sobre el motor real) con **minimax + poda alfa-beta** una vez que la partida se sale del libro. El libro además "aprende": después de cada partida actualiza el historial de victorias/derrotas de las líneas jugadas.
- **3 niveles de dificultad**: Fácil (movimientos al azar fuera del libro — pensado para que un principiante pueda ganar), Difícil y Profesional (búsqueda más profunda, siempre la mejor línea del libro).
- **Frontend en Blazor WebAssembly**: tablero interactivo, selector de nivel, historial de jugadas configurable (nombre de pieza / coordenadas / ambos), animación de movimiento de piezas.
- **Persistencia relacional real** en SQL Server (LocalDB), sin blobs JSON — cada partida se guarda en tablas normalizadas.

## Arquitectura

Clean Architecture con 5 capas, cada una dependiendo solo de las que están "adentro" suyo:

```
Domain          → sin dependencias externas. Entidades, value objects,
                  Specification pattern (reglas de movimiento por pieza),
                  Domain Events, servicios de dominio.
Application     → depende de Domain. Casos de uso con CQRS (MediatR):
                  comandos y queries, validación con FluentValidation.
Infrastructure  → depende de Application. Persistencia (EF Core + SQL
                  Server) y el motor de IA (libro de aperturas + minimax).
AjedrezApi      → depende de Application e Infrastructure. Controllers
                  REST delgados: solo orquestan MediatR.
AjedrezWeb      → depende solo de Ajedrez.Shared. Blazor WebAssembly,
                  consume la API por HTTP.
Ajedrez.Shared  → DTOs compartidos entre la API y el frontend, sin
                  referencia a Domain (para que el cliente WASM sea liviano).
```

```
Clean_Arquitecture.slnx
├── Domain/                    Entidades, value objects, Specification, Domain Events
├── Application/                Commands, Queries, validadores (MediatR + FluentValidation)
├── Infrastructure/             EF Core (persistencia relacional), motor de IA
├── Ajedrez.Shared/              DTOs compartidos API ↔ Blazor
├── AjedrezApi/                  Web API (ASP.NET Core, controllers)
├── AjedrezWeb/                  Blazor WebAssembly (frontend)
└── Tests/
    ├── Ajedrez.Domain.Tests/
    ├── Ajedrez.Application.Tests/
    └── Ajedrez.Infrastructure.Tests/
```

## Patrones y prácticas usados

- **Specification pattern** — cada tipo de pieza tiene su propia clase (`EspecificacionMovimientoPeon`, `EspecificacionMovimientoCaballo`, etc.) que sabe generar sus movimientos candidatos; las piezas que deslizan (torre, alfil, dama) comparten una base común.
- **Domain Events** — `JaqueDetectadoEvent`, `JaqueMateDetectadoEvent`, `PartidaTerminadaEvent`, etc. se disparan dentro del aggregate `Partida` y se republican vía MediatR (`DomainEventNotificacion<T>`) sin que el Domain dependa de MediatR.
- **CQRS con MediatR** — comandos (`HacerMovimiento`, `IniciarPartida`, `ObtenerJugadaIA`) y queries (`ObtenerEstadoPartida`, `ObtenerMovimientosLegales`) separados, con un pipeline behavior de validación.
- **Repository pattern** — `IPartidaRepository` definido en Application, implementado en Infrastructure sobre un esquema relacional (no un blob serializado).
- **Aggregate root con reconstitución** — `Partida.Reconstruir(...)` rehidrata el aggregate desde la base de datos sin volver a disparar los Domain Events (que ya ocurrieron la primera vez que se jugó cada movimiento).

## Cómo levantarlo (Visual Studio)

1. Abre `Clean_Arquitecture.slnx`.
2. Clic derecho sobre la solución → **Configurar proyectos de inicio** → **Varios proyectos de inicio** → pon `AjedrezApi` y `AjedrezWeb` en **Inicio**, ambos con el perfil **http** (no https).
3. **F5** o **Ctrl+F5**.

No hace falta preparar la base de datos a mano: `AjedrezApi` aplica las migraciones de EF Core en SQL Server LocalDB (y siembra el libro de aperturas) automáticamente al arrancar si no existen.

### Esquema de la base de datos

El esquema está versionado como migraciones de EF Core en `Infrastructure/Persistencia/Migraciones/`. Si prefieres crear la base a mano (sin correr la app), el script SQL equivalente —generado directamente desde la migración con `dotnet ef migrations script --idempotent`, así que siempre coincide con el modelo real— está en [`Scripts/esquema-base-datos.sql`](Scripts/esquema-base-datos.sql).

## Tests

41 pruebas automatizadas repartidas en 3 proyectos:

- **Domain** (19): reglas de movimiento, jaque mate, ahogado, enroque, captura al paso, repetición triple, material insuficiente.
- **Application** (9): comandos y queries contra un repositorio falso en memoria.
- **Infrastructure** (13): persistencia contra SQL Server real, motor minimax (incluye un test de mate en 1), selección del libro de aperturas por nivel.

```bash
dotnet test
```

## Stack

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=flat-square)
![Blazor WebAssembly](https://img.shields.io/badge/Blazor_WASM-512BD4?style=flat-square&logo=blazor&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square)
![MediatR](https://img.shields.io/badge/MediatR-1E6FD9?style=flat-square)
![FluentValidation](https://img.shields.io/badge/FluentValidation-4B32C3?style=flat-square)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=flat-square)
![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-2088FF?style=flat-square&logo=githubactions&logoColor=white)
