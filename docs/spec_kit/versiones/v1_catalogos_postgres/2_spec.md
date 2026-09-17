# 2_spec.md — Entrega 1: catálogos sin llave foránea

> Rige junto con [1_constitution.md](../../1_constitution.md). Ante
> conflicto, la constitución gana.

## Propósito

Exponer, mediante API REST, el CRUD con borrado lógico de las 7 tablas de
catálogo del módulo que no tienen llaves foráneas, para que puedan
alimentarse y consultarse antes de construir las entidades que dependen
de ellas.

## Alcance

**Incluye:** CRUD completo (crear, listar, obtener por id, actualizar,
eliminar lógico) de `area_conocimiento`, `universidad`,
`aspecto_normativo`, `practica_estrategia`, `enfoque`, `car_innovacion`,
`aliado`. Validación de llave repetida al crear. Validación de longitud
de campos según `db/00_innovacion_curricular.pg.sql`. Interfaz web con
listado, creación, edición y eliminación por tabla.

**NO incluye:**

| Excluido | Entrega que lo cubre |
|---|---|
| Tablas con llave foránea (`facultad`, `programa`, `acreditacion`, …) | Entrega 2 |
| Autenticación, JWT, roles, login | Entrega 3 |
| Reportes multitabla, dashboard, portal corporativo, despliegue | Entrega 4 |
| Generación automática de la llave primaria | nunca (Artículo 8 de la constitución: es `INT`, no `SERIAL`) |

## Requisitos funcionales

| # | Requisito |
|---|---|
| RF1 | Listar: cada tabla expone sus registros con `activo = true`; los inactivos no aparecen. |
| RF2 | Obtener por id: consulta puntual por llave primaria (`id`, o `nit` en `aliado`); si no existe o está inactivo, se informa que no fue encontrado. |
| RF3 | Crear: recibe la llave primaria y los campos propios; se rechaza si la llave ya existe o si un campo excede su longitud máxima. |
| RF4 | Actualizar: modifica los campos propios (no la llave primaria) de un registro existente; se rechaza si no existe o está inactivo. |
| RF5 | Eliminar (lógico): marca `activo = false`; nunca ejecuta `DELETE`; se rechaza si el registro no existe o ya está inactivo. |
| RF6 | Interfaz web: por cada tabla, pantalla de listado + formularios de creación y edición, consumiendo únicamente la API. |

## Criterios de aceptación

1. `GET /api/area_conocimiento` → `200`, arreglo que solo contiene
   `activo: true`.
2. Crear con un id ya existente → `400` con mensaje de duplicado.
3. Crear con un campo que excede su longitud máxima → `400` de
   validación.
4. Ciclo crear → listar (aparece) → editar (cambia) → eliminar → listar
   (ya no aparece), sin error, en las 7 tablas.
5. Tras "eliminar", el registro sigue en la base de datos con
   `activo = false` — verificable por SQL directo, no por la API.
6. El frontend completa el ciclo del criterio 4, para las 7 tablas, sin
   usar Swagger.

## Historial de cambios

| Versión | Fecha | Cambio |
|---|---|---|
| 1.0 | (reconstruida junto con el spec kit) | Primera versión formal, documenta la Entrega 1 tal como quedó implementada |
