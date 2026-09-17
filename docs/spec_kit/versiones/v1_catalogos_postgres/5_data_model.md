# 5_data_model.md — Versión 1: catálogos sin llave foránea

## Tablas que toca esta versión

Las 7, todas dadas por `db/00_innovacion_curricular.pg.sql` (sin
modificar, artículo 5) más la columna `activo` que agrega
`db/01_alter_activo.sql` (decisión D2).

### `area_conocimiento`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria, la envía el cliente (D1) |
| gran_area | VARCHAR(60) | obligatorio |
| area | VARCHAR(60) | obligatorio |
| disciplina | VARCHAR(60) | obligatorio |
| activo | BOOLEAN | agregada por 01_alter_activo.sql, default true |

### `universidad`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria |
| nombre | VARCHAR(60) | obligatorio |
| tipo | VARCHAR(45) | obligatorio |
| ciudad | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

### `aspecto_normativo`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria |
| tipo | VARCHAR(45) | obligatorio |
| descripcion | VARCHAR(45) | obligatorio |
| fuente | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

### `practica_estrategia`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria |
| tipo | VARCHAR(45) | obligatorio |
| nombre | VARCHAR(45) | obligatorio |
| descripcion | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

### `enfoque`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria |
| nombre | VARCHAR(45) | obligatorio |
| descripcion | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

### `car_innovacion`
| Columna | Tipo | Notas |
|---|---|---|
| id | INT | llave primaria |
| nombre | VARCHAR(45) | obligatorio |
| descripcion | TEXT | obligatorio, sin límite de longitud |
| tipo | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

### `aliado`
| Columna | Tipo | Notas |
|---|---|---|
| nit | INT | **llave primaria** (no se llama `id`, decisión D6) |
| razon_social | VARCHAR(60) | obligatorio |
| nombre_contacto | VARCHAR(60) | obligatorio |
| correo | VARCHAR(70) | obligatorio |
| telefono | VARCHAR(45) | obligatorio |
| ciudad | VARCHAR(45) | obligatorio |
| activo | BOOLEAN | agregada |

## Qué calcula la base de datos y qué no

- La base de datos **no** calcula ni genera ninguna llave primaria de
  estas 7 tablas (no son `SERIAL`) — la API debe enviarla siempre.
- `activo` tiene `DEFAULT TRUE`: si la API inserta sin mencionar esa
  columna, la base de datos la deja en `true` sola. Aun así, la API
  fija el valor explícitamente (`true`) al crear, por claridad.
- Ningún trigger ni procedimiento almacenado existe sobre estas tablas.

## Lo que la API tiene PROHIBIDO hacer

- Ejecutar `DELETE FROM` sobre cualquiera de estas tablas (artículo 4).
- Modificar la definición de las columnas (`ALTER TABLE` fuera de los
  scripts numerados de `db/`, artículo 5).
- Insertar un registro sin verificar antes que su llave primaria no
  exista (D1).

## Datos de referencia

Estas 7 tablas se alimentan, en producción, con los datos de referencia
que provee el profesor (por ejemplo, 218 filas para `area_conocimiento`,
6 para `universidad`). Al momento de escribir este documento, esa carga
masiva **no se ha ejecutado todavía** — la entrega se valida con
registros de prueba creados manualmente vía la API/frontend, y queda
pendiente para cuando el equipo reciba el archivo oficial.
