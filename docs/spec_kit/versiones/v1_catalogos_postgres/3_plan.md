# 3_plan.md — Versión 1: catálogos sin llave foránea

## Stack

- **Backend:** C# sobre ASP.NET Core (.NET 10).
- **Acceso a datos:** Dapper (SQL parametrizado, sin ORM completo — artículo 3).
- **Base de datos:** PostgreSQL 16, base `innovacion_curricular`.
- **Frontend:** ASP.NET Core Razor Pages (mismo lenguaje que el backend).
- **Documentación interactiva de la API:** Swagger (Swashbuckle.AspNetCore).
- **Contenedores:** Docker + docker-compose (tres servicios: `db`, `api`, `frontend`).

## Estructura de carpetas

```
proyecto/
├── docker-compose.yml
├── db/
│   ├── 00_innovacion_curricular.pg.sql   ← dado por el profesor, sin tocar
│   └── 01_alter_activo.sql               ← agregado (artículo 5): columna activo
├── docs/
│   └── spec_kit/
│       ├── 1_constitution.md
│       └── versiones/
│           └── v1_catalogos_postgres/    ← este spec kit
├── api_innovacion/
│   ├── ApiInnovacionCurricular.csproj
│   ├── Program.cs
│   ├── Modelos/           ← una clase por tabla (7 archivos)
│   ├── Peticiones/        ← Crear.../Actualizar... por tabla (7 archivos)
│   ├── Controllers/       ← un Controller por tabla (7 archivos)
│   ├── Servicios/         ← interfaz + implementación por tabla (7 pares)
│   ├── Repositorios/      ← interfaz + implementación por tabla (7 pares)
│   └── Excepciones/       ← NoEncontradoExcepcion, ConflictoExcepcion (compartidas)
└── frontend_innovacion/
    ├── FrontendInnovacionCurricular.csproj
    ├── Program.cs
    ├── Modelos/            ← mismas 7 clases, del lado del frontend
    ├── Servicios/          ← un "cliente HTTP" por tabla (7 archivos)
    └── Pages/
        ├── Shared/_Layout.cshtml
        └── {NombreTabla}/  ← Index.cshtml(+.cs), Crear.cshtml(+.cs), Editar.cshtml(+.cs)
```

## Diseño de capas (qué hace cada una, por tabla)

Las 7 tablas siguen exactamente el mismo patrón de 4 capas. Se documenta
una vez porque se repite idéntico en las 7:

1. **Modelo** (`Modelos/{Tabla}.cs`) — clase con una propiedad por columna,
   incluida `Activo`.
2. **Petición** (`Peticiones/{Tabla}Peticiones.cs`) — dos clases,
   `Crear{Tabla}Peticion` (con id) y `Actualizar{Tabla}Peticion` (sin id),
   con validaciones `[Required]` y `[MaxLength]` que reflejan las
   restricciones del script SQL original.
3. **Repositorio** (`Repositorios/{Tabla}Repositorio.cs`) — interfaz
   `I{Tabla}Repositorio` + implementación con Dapper: `ListarAsync`,
   `ObtenerPorIdAsync`, `ExisteIdAsync`, `CrearAsync`, `ActualizarAsync`,
   `EliminarLogicoAsync`.
4. **Servicio** (`Servicios/{Tabla}Servicio.cs`) — interfaz
   `I{Tabla}Servicio` + implementación: aplica la regla de "id no
   repetido" al crear, y lanza `NoEncontradoExcepcion` si el registro no
   existe en actualizar/eliminar/obtener.
5. **Controller** (`Controllers/{Tabla}Controller.cs`) — expone
   `GET /api/{tabla}`, `GET /api/{tabla}/{id}`, `POST /api/{tabla}`,
   `PUT /api/{tabla}/{id}`, `DELETE /api/{tabla}/{id}`; traduce las
   excepciones del servicio a códigos HTTP (400/404) según
   `6_contracts.md`.

Excepción de nomenclatura: **`aliado`** no tiene columna `id` sino `nit`
como llave primaria — sus métodos y rutas usan `{nit}` en vez de `{id}`,
pero las cuatro capas siguen el mismo patrón.

Del lado del frontend, cada tabla tiene un **cliente HTTP**
(`Servicios/{Tabla}Cliente.cs`) que es el único punto que le habla a la
API (`HttpClient`), y tres páginas Razor (`Index`, `Crear`, `Editar`) que
usan ese cliente — nunca hacen `HttpClient` directo ni tocan la base de
datos (artículo 1).

## Inventario de archivos por tabla (se repite ×7)

| Capa | Archivo backend | Archivo frontend |
|---|---|---|
| Modelo | `Modelos/{Tabla}.cs` | `Modelos/{Tabla}.cs` |
| Petición | `Peticiones/{Tabla}Peticiones.cs` | — |
| Repositorio | `Repositorios/{Tabla}Repositorio.cs` | — |
| Servicio | `Servicios/{Tabla}Servicio.cs` | `Servicios/{Tabla}Cliente.cs` |
| Controller | `Controllers/{Tabla}Controller.cs` | — |
| Páginas | — | `Pages/{Tabla}/Index.cshtml(+.cs)`, `Crear.cshtml(+.cs)`, `Editar.cshtml(+.cs)` |
