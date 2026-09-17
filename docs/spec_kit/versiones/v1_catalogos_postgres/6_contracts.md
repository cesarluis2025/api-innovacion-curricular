# 6_contracts.md — Entrega 1: catálogos sin llave foránea

> Formato de errores y códigos según el Artículo 8 de la constitución.

## `area_conocimiento` (documentado completo; representa a las 6 tablas con `id`)

| Verbo y ruta | Body | Éxito | Errores |
|---|---|---|---|
| `GET /api/area_conocimiento` | — | `200` arreglo `[{id, granArea, area, disciplina, activo}]` | — |
| `GET /api/area_conocimiento/{id}` | — | `200` el objeto | `404` `{ "mensaje": "no existe un área de conocimiento con id {id}" }` |
| `POST /api/area_conocimiento` | `{id, granArea, area, disciplina}` | `201` objeto creado, header `Location` | `400` duplicado: `{ "mensaje": "ya existe un área de conocimiento con id {id}" }` · `400` validación: `ValidationProblemDetails` |
| `PUT /api/area_conocimiento/{id}` | `{granArea, area, disciplina}` | `200` objeto actualizado | `404` igual al `GET` |
| `DELETE /api/area_conocimiento/{id}` | — | `204` (marca `activo=false`) | `404` igual al `GET` |

## `aliado` (llave primaria `nit`, no `id` — decisión D6)

| Verbo y ruta | Body | Éxito | Errores |
|---|---|---|---|
| `GET /api/aliado` | — | `200` arreglo `[{nit, razonSocial, nombreContacto, correo, telefono, ciudad, activo}]` | — |
| `GET /api/aliado/{nit}` | — | `200` el objeto | `404` `{ "mensaje": "no existe un aliado con nit {nit}" }` |
| `POST /api/aliado` | `{nit, razonSocial, nombreContacto, correo, telefono, ciudad}` | `201` | `400` duplicado / validación |
| `PUT /api/aliado/{nit}` | sin `nit` | `200` | `404` |
| `DELETE /api/aliado/{nit}` | — | `204` | `404` |

## Las otras 5 tablas — mismo patrón, mismos códigos (200/201/204/400/404)

| Tabla | Ruta base | Campos del body (sin id) |
|---|---|---|
| `universidad` | `/api/universidad` | `nombre`, `tipo`, `ciudad` |
| `aspecto_normativo` | `/api/aspecto_normativo` | `tipo`, `descripcion`, `fuente` |
| `practica_estrategia` | `/api/practica_estrategia` | `tipo`, `nombre`, `descripcion` |
| `enfoque` | `/api/enfoque` | `nombre`, `descripcion` |
| `car_innovacion` | `/api/car_innovacion` | `nombre`, `descripcion`, `tipo` |

El mensaje `"ya existe [tabla] con id {id}"` / `"no existe [tabla] con id
{id}"` cambia solo el nombre de la entidad; el formato es idéntico en las
7 tablas.
