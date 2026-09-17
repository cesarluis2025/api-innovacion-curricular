# 6_contracts.md — Versión 1: catálogos sin llave foránea

Las 7 tablas exponen el mismo patrón de 5 endpoints. Se documenta
completo para `area_conocimiento` (representativa de las 6 que usan
`id`) y para `aliado` (representativa por usar `nit`); las otras 5 solo
listan sus rutas y campos propios, porque los códigos de respuesta y el
formato son idénticos.

---

## `area_conocimiento`

### `GET /api/area_conocimiento`
- **200 OK**
```json
[
  { "id": 1, "granArea": "...", "area": "...", "disciplina": "...", "activo": true }
]
```

### `GET /api/area_conocimiento/{id}`
- **200 OK** — el objeto tal como arriba.
- **404 Not Found**
```json
{ "mensaje": "no existe un área de conocimiento con id {id}" }
```

### `POST /api/area_conocimiento`
Body:
```json
{ "id": 1, "granArea": "...", "area": "...", "disciplina": "..." }
```
- **201 Created** — devuelve el objeto creado, header `Location` apuntando a `GET /api/area_conocimiento/{id}`.
- **400 Bad Request** (id repetido)
```json
{ "mensaje": "ya existe un área de conocimiento con id {id}" }
```
- **400 Bad Request** (validación: campo vacío o más largo que el máximo)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "GranArea": ["..."] }
}
```

### `PUT /api/area_conocimiento/{id}`
Body: igual al de creación, sin `id`.
- **200 OK** — el objeto actualizado.
- **404 Not Found** — mismo formato del `GET` por id.

### `DELETE /api/area_conocimiento/{id}`
- **204 No Content** — el registro queda con `activo = false`.
- **404 Not Found** — mismo formato del `GET` por id (aplica si ya estaba inactivo o nunca existió).

---

## `aliado` (llave primaria `nit`, no `id`)

### `GET /api/aliado`
- **200 OK**
```json
[
  { "nit": 900123456, "razonSocial": "...", "nombreContacto": "...", "correo": "...", "telefono": "...", "ciudad": "...", "activo": true }
]
```

### `GET /api/aliado/{nit}`
- **200 OK** / **404 Not Found** — `{ "mensaje": "no existe un aliado con nit {nit}" }`

### `POST /api/aliado`
Body: `{ "nit": ..., "razonSocial": "...", "nombreContacto": "...", "correo": "...", "telefono": "...", "ciudad": "..." }`
- **201 Created** / **400 Bad Request** — `{ "mensaje": "ya existe un aliado con nit {nit}" }` o error de validación (mismo formato que `area_conocimiento`).

### `PUT /api/aliado/{nit}`
Body: igual sin `nit`. **200 OK** / **404 Not Found**.

### `DELETE /api/aliado/{nit}`
**204 No Content** / **404 Not Found**.

---

## Las otras 5 tablas (mismo patrón, mismos códigos 200/201/204/400/404)

| Tabla | Ruta base | Campos del body (POST/PUT, sin el id) |
|---|---|---|
| `universidad` | `/api/universidad` | `nombre`, `tipo`, `ciudad` |
| `aspecto_normativo` | `/api/aspecto_normativo` | `tipo`, `descripcion`, `fuente` |
| `practica_estrategia` | `/api/practica_estrategia` | `tipo`, `nombre`, `descripcion` |
| `enfoque` | `/api/enfoque` | `nombre`, `descripcion` |
| `car_innovacion` | `/api/car_innovacion` | `nombre`, `descripcion`, `tipo` |

Para cada una: `GET /{ruta}` (200, arreglo), `GET /{ruta}/{id}` (200 o 404),
`POST /{ruta}` (201 con el objeto creado, o 400 con `{ "mensaje": "ya
existe [tabla] con id {id}" }`, o 400 de validación), `PUT /{ruta}/{id}`
(200 o 404), `DELETE /{ruta}/{id}` (204 o 404). El mensaje de "no existe"
sigue el mismo formato, cambiando solo el nombre de la entidad.
