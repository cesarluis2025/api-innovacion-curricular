# 3_plan.md — Entrega 1: catálogos sin llave foránea

> Aplica el Artículo 2 (stack) y el Artículo 3 (capas) de la
> constitución a esta entrega concreta.

## Stack de esta entrega

| Capa | Tecnología |
|---|---|
| Backend | C# / ASP.NET Core (.NET 10) |
| Acceso a datos | Dapper, SQL parametrizado (Artículo 2) |
| Base de datos | PostgreSQL 16, base `innovacion_curricular` |
| Frontend | ASP.NET Core Razor Pages |
| Documentación de la API | Swagger (Swashbuckle.AspNetCore) |
| Orquestación | Docker + docker-compose (servicios `db`, `api`, `frontend`) |

## Estructura de carpetas

```
proyecto/
├── docker-compose.yml
├── db/
│   ├── 00_innovacion_curricular.pg.sql   ← dado, no se toca (Artículo 5)
│   └── 01_alter_activo.sql               ← agregado: columna activo
├── docs/spec_kit/                        ← este spec kit
├── api_innovacion/
│   ├── ApiInnovacionCurricular.csproj
│   ├── Program.cs                        ← único lugar con clases concretas (Artículo 3)
│   ├── Modelos/        (7 archivos)
│   ├── Peticiones/     (7 archivos)
│   ├── Controllers/    (7 archivos)
│   ├── Servicios/      (7 interfaces + 7 implementaciones)
│   ├── Repositorios/   (7 interfaces + 7 implementaciones)
│   └── Excepciones/    (2 archivos, compartidos)
└── frontend_innovacion/
    ├── FrontendInnovacionCurricular.csproj
    ├── Program.cs
    ├── Modelos/         (7 archivos, espejo de la API)
    ├── Servicios/       (7 clientes HTTP)
    └── Pages/
        ├── Shared/_Layout.cshtml
        └── {Tabla}/     Index · Crear · Editar (.cshtml + .cshtml.cs)
```

## Diseño de capas por tabla (idéntico en las 7, Artículo 3)

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Modelo | `Modelos/{Tabla}.cs` | una propiedad por columna, incluida `Activo` |
| Petición | `Peticiones/{Tabla}Peticiones.cs` | `Crear{Tabla}Peticion` (con id) y `Actualizar{Tabla}Peticion` (sin id), con `[Required]`/`[MaxLength]` |
| Repositorio | `Repositorios/{Tabla}Repositorio.cs` | `ListarAsync`, `ObtenerPorIdAsync`, `ExisteIdAsync`, `CrearAsync`, `ActualizarAsync`, `EliminarLogicoAsync` — SQL con Dapper |
| Servicio | `Servicios/{Tabla}Servicio.cs` | valida id no repetido al crear; lanza `NoEncontradoExcepcion` si no existe |
| Controller | `Controllers/{Tabla}Controller.cs` | expone las rutas de `6_contracts.md`; traduce excepciones a HTTP |

Excepción de nomenclatura: **`aliado`** usa `nit` como llave primaria en
vez de `id` en todas sus capas y rutas (ver decisión D6 en
`4_research.md`).

Del lado del frontend, cada tabla tiene un cliente HTTP
(`Servicios/{Tabla}Cliente.cs`) — el único punto que le habla a la API —
y tres páginas Razor que lo usan, nunca `HttpClient` directo (Artículo 1
de la constitución: separación estricta backend/frontend).
