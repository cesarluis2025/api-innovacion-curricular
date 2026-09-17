# 2_spec.md — Versión 1: catálogos sin llave foránea

Módulo: innovación curricular. Base de datos: `innovacion_curricular`.

## Propósito

Exponer, mediante una API REST, el CRUD con borrado lógico de las 7 tablas
de catálogo del módulo que no tienen llaves foráneas, para que puedan
alimentarse y consultarse antes de construir las entidades que dependen de
ellas.

## Alcance

**Incluye:**
- CRUD completo (crear, listar, obtener por id, actualizar, eliminar
  lógico) de las 7 tablas: `area_conocimiento`, `universidad`,
  `aspecto_normativo`, `practica_estrategia`, `enfoque`, `car_innovacion`,
  `aliado`.
- Validación de duplicados por llave primaria al crear.
- Validación de longitud de campos según el script de base de datos
  original del profesor.
- Interfaz web (frontend) con listado, creación, edición y eliminación
  para cada una de las 7 tablas.

**NO incluye** (queda para versiones posteriores):
- Ninguna tabla con llave foránea (`facultad`, `programa`, `acreditacion`,
  etc. — son la Entrega 2).
- Autenticación, JWT, roles o login (Entrega 3).
- Reportes multitabla, dashboard, portal corporativo o despliegue en
  producción (Entrega 4).
- Generación automática de la llave primaria: el script de base de datos
  entregado por el profesor la define como `INT` sin autoincremento, así
  que el id se recibe en la petición de creación, no se genera solo (ver
  `4_research.md`, decisión D1).

## Requisitos funcionales

### RF1 — Listar
Cada tabla expone un listado de sus registros con `activo = true`.
Los registros marcados inactivos no aparecen.

### RF2 — Obtener por id
Cada tabla permite consultar un registro puntual por su llave primaria
(`id`, salvo `aliado` que usa `nit`). Si no existe o está inactivo, se
informa que no fue encontrado.

### RF3 — Crear
Cada tabla permite crear un registro nuevo, recibiendo su llave primaria
y sus campos propios. Se rechaza si la llave primaria ya existe, o si
algún campo excede la longitud máxima definida en el script de base de
datos original.

### RF4 — Actualizar
Cada tabla permite actualizar los campos propios de un registro existente
(no su llave primaria). Se rechaza si el registro no existe o está
inactivo.

### RF5 — Eliminar (lógico)
Cada tabla permite dar de baja un registro sin borrarlo físicamente:
se actualiza su columna `activo` a `false`. Se rechaza si el registro no
existe o ya está inactivo.

### RF6 — Interfaz web
Para cada una de las 7 tablas existe una pantalla que lista sus registros
activos, y formularios para crear y editar, consumiendo únicamente la API
(nunca la base de datos directamente).

## Criterios de aceptación

1. `GET /api/area_conocimiento` responde `200` con un arreglo JSON que
   solo contiene registros con `activo: true`.
2. Crear un registro con un id que ya existe responde `400` con un mensaje
   que indica que el id ya existe.
3. Crear un registro con un campo que excede su longitud máxima (por
   ejemplo, `descripcion` de `enfoque` con más de 45 caracteres) responde
   `400` con el detalle de validación.
4. El ciclo completo crear → listar (aparece) → editar (cambia) → eliminar
   → listar (ya no aparece) funciona sin error en las 7 tablas.
5. Después de "eliminar" un registro, sigue existiendo en la base de
   datos con `activo = false` (verificable por SQL directo) — nunca se
   ejecuta un `DELETE` físico.
6. El frontend, para cada una de las 7 tablas, permite completar el ciclo
   del criterio 4 sin usar Swagger ni ninguna herramienta aparte.

## Historial de cambios

| Versión | Fecha | Cambio |
|---|---|---|
| 1.0 | (reconstruida a partir del código ya construido) | Primera versión formal del spec kit; documenta la Entrega 1 tal como quedó implementada |
