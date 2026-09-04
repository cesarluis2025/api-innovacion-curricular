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
- **frontend**: la interfaz web → http://localhost:8081

## Estructura

- `db/` — scripts SQL que se ejecutan al crear la base de datos
- `api_innovacion/` — backend en C#/ASP.NET Core (Controllers → Servicios → Repositorios → Modelos)
- `frontend_innovacion/` — frontend en Razor Pages, consume la API por HTTP (nunca toca la base de datos directo)
- `docs/spec_kit/` — constitución y documentación de la metodología del proyecto

## Estado — Entrega 1 (catálogos sin llave foránea)

Tablas con CRUD completo (crear, listar, editar, eliminar lógico) en API y frontend:

- [x] área_conocimiento
- [x] universidad
- [x] aspecto_normativo
- [x] practica_estrategia
- [x] enfoque
- [x] car_innovacion
- [x] aliado

Pendiente para dejar la Entrega 1 100% completa:

- [ ] Repositorio de GitHub independiente para `frontend_innovacion/` (la API ya tiene el suyo)
- [ ] Que Cesar cree su propia rama en el repositorio de la API
- [ ] Cargar los datos de referencia reales del profesor (218 área_conocimiento, 6 universidad, etc.)

