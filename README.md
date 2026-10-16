# Módulo de innovación curricular

Sistema de gestión de información del conocimiento universitario — módulo de innovación curricular.
Equipo: Carlos Adrián Rentería Machado, Cesar Luis Mosquera García. Diseño de Software, USB Medellín.

## Cómo correr el proyecto

Requisitos: Docker Desktop instalado y corriendo.

```
Copy-Item env.example.txt .env     # y edite .env: ponga su propia DB_PASSWORD
docker compose up -d --build
```

Esto levanta tres contenedores:

- **db**: PostgreSQL con la base `innovacion_curricular` (script del profesor, columnas nuevas, 115 funciones `sp_*`, disparador y datos de referencia)
- **api**: la API REST → http://localhost:8080 (documentación interactiva en http://localhost:8080/swagger)
- **frontend**: la interfaz web → http://localhost:8081

La primera vez, espere 60 a 90 segundos a que `dotnet watch` compile.

## Estructura

- `db/` — scripts SQL que se ejecutan al crear la base de datos, en orden numérico
- `api_innovacion/` — backend en C#/ASP.NET Core (Controllers → Servicios → Repositorios → Modelos); todo el acceso a datos pasa por funciones `sp_*`
- `frontend_innovacion/` — frontend en Razor Pages, consume la API por HTTP (nunca toca la base de datos directo)
- `pruebas/` — `v2_pruebas.ps1` corre los 12 criterios y la regresión de la v1; `evidencia_v2.txt` es su última salida
- `docs/spec_kit/` — constitución y documentación de la metodología del proyecto (spec kit de cada versión)

## Estado — Versión 2 (las 22 tablas del módulo)

- [x] Las 22 tablas con CRUD completo en API e interfaz (borrado lógico con `activo`)
- [x] Todos los verbos por procedimientos almacenados (115 funciones)
- [x] Disparador: `facultad.total_programas` se mantiene solo
- [x] Las llaves foráneas son desplegables con nombres, cargados desde la API; las opcionales aceptan «(ninguna)»
- [x] Tablas puente: asignar, repetir (409), retirar y reactivar
- [x] Maestro-detalle: varias filas en una sola petición, todo o nada
- [x] Hijo con padre inexistente o llave repetida → 409, nunca 500
- [x] Regresión de la v1
- [ ] Merge a main y tag `v2` (lo hace el encargado del main, en los dos repositorios)

Para verificar: `powershell -ExecutionPolicy Bypass -File .\pruebas\v2_pruebas.ps1`
