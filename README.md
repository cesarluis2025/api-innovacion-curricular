# Módulo de innovación curricular

Sistema de gestión de información del conocimiento universitario — módulo de innovación curricular.
Equipo: Carlos Adrián Rentería Machado, Cesar Luis Mosquera García. Diseño de Software, USB Medellín.

## Cómo correr el proyecto

Requisitos: Docker Desktop instalado y corriendo.

```
docker compose up -d --build
```

Esto levanta tres contenedores:

- **db**: PostgreSQL con la base `innovacion_curricular` (script del profesor + columna `activo` agregada)
- **api**: la API REST → http://localhost:8080 (documentación interactiva en http://localhost:8080/swagger)
- **frontend**: la interfaz web → http://localhost:8081/AreaConocimiento

## Estructura

- `db/` — scripts SQL que se ejecutan al crear la base de datos
  - `00_innovacion_curricular.pg.sql` — el script original del profesor, sin modificar
  - `01_alter_activo.sql` — agrega la columna `activo` a las 22 tablas del módulo (el script original no la traía)
- `api_innovacion/` — backend en C#/ASP.NET Core (Controllers → Servicios → Repositorios → Modelos), acceso a datos con Dapper
- `frontend_innovacion/` — frontend en Razor Pages (C#/ASP.NET Core), consume la API por HTTP; nunca toca la base de datos directo
- `docker-compose.yml` — orquesta los tres servicios juntos

## Decisiones técnicas

- **Borrado lógico**: ninguna operación hace `DELETE`; eliminar marca `activo = false`. Los listados solo muestran `activo = true`.
- **ID manual**: las llaves primarias del script original son `INT` sin autoincremento (no `SERIAL`), así que el ID se digita en el formulario de creación, no se genera solo.
- **Separación de repositorios**: `api_innovacion` y `frontend_innovacion` viven en repositorios de GitHub independientes, comunicándose solo por HTTP/JSON.

## Repositorios

- API: https://github.com/cesarluis2025/api-innovacion-curricular
- Frontend: https://github.com/cesarluis2025/frontend-innovacion-curricular

Cada repositorio tiene tres ramas: `main`, `carlos-dev`, `cesar-dev`.

## Estado — Entrega 1 (catálogos sin llave foránea)

Tablas con CRUD completo (crear, listar, editar, eliminar lógico) en API y frontend:

- [x] área_conocimiento
- [x] universidad
- [x] aspecto_normativo
- [x] practica_estrategia
- [x] enfoque
- [x] car_innovacion
- [x] aliado

Datos cargados actualmente: registros de prueba creados manualmente vía Swagger, para demostrar el CRUD.
Pendiente: cargar los datos de referencia oficiales del profesor (218 área_conocimiento, 6 universidad, etc.) cuando se disponga del archivo Excel.
