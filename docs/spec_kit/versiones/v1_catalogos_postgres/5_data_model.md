# 5_data_model.md — Entrega 1: catálogos sin llave foránea

Las 7 tablas vienen dadas por `db/00_innovacion_curricular.pg.sql`
(Artículo 5, sin modificar) más la columna `activo` que agrega
`db/01_alter_activo.sql` (decisión D2).

| Tabla | Llave primaria | Campos propios (VARCHAR salvo lo indicado) |
|---|---|---|
| `area_conocimiento` | `id` INT | `gran_area`(60), `area`(60), `disciplina`(60) |
| `universidad` | `id` INT | `nombre`(60), `tipo`(45), `ciudad`(45) |
| `aspecto_normativo` | `id` INT | `tipo`(45), `descripcion`(45), `fuente`(45) |
| `practica_estrategia` | `id` INT | `tipo`(45), `nombre`(45), `descripcion`(45) |
| `enfoque` | `id` INT | `nombre`(45), `descripcion`(45) |
| `car_innovacion` | `id` INT | `nombre`(45), `descripcion` TEXT (sin límite), `tipo`(45) |
| `aliado` | **`nit`** INT (no `id`, ver D6) | `razon_social`(60), `nombre_contacto`(60), `correo`(70), `telefono`(45), `ciudad`(45) |

Todas obligatorias (`NOT NULL`) en el script original. `activo` BOOLEAN,
`DEFAULT TRUE`, agregada en `01_alter_activo.sql`.

## Qué calcula la base de datos y qué no

- Ninguna de estas 7 tablas genera su llave primaria (no son `SERIAL`,
  Artículo 8) — la API debe enviarla siempre.
- `activo` tiene `DEFAULT TRUE`; aun así, la API lo fija explícitamente
  al crear.
- Sin triggers ni procedimientos almacenados sobre estas tablas.

## Prohibido para la API (Artículo 4/5 de la constitución)

- `DELETE FROM` sobre cualquiera de estas tablas.
- `ALTER TABLE` fuera de los scripts numerados de `db/`.
- Insertar sin verificar antes que la llave primaria no exista (D1).

## Datos de referencia

En producción, estas 7 tablas se alimentan con los datos de referencia
del profesor (por ejemplo, 218 filas para `area_conocimiento`). Al cierre
de esta entrega, esa carga masiva **no se ha ejecutado**: la entrega se
valida con registros de prueba creados manualmente, y queda pendiente
para cuando el equipo reciba el archivo oficial.
