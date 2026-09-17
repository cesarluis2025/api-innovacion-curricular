# 7_quickstart.md — Entrega 1: catálogos sin llave foránea

## Arranque (Artículo 4 de la constitución: un solo comando)

```powershell
docker compose up -d --build
```

- API: `http://localhost:8080` · Swagger: `http://localhost:8080/swagger`
- Frontend: `http://localhost:8081`

## Smoke test — recorre los 6 criterios de `2_spec.md`

Con `area_conocimiento` como tabla de prueba; el mismo patrón aplica a
las otras 6.

```powershell
# 1. Listar (criterio 1) — 200, arreglo
curl.exe http://localhost:8080/api/area_conocimiento

# 2. Crear (preparación)
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":90,"granArea":"Prueba","area":"Prueba","disciplina":"Prueba"}'
# esperado: 201

# 3. Duplicado (criterio 2)
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":90,"granArea":"Otra","area":"Otra","disciplina":"Otra"}'
# esperado: 400, "ya existe un área de conocimiento con id 90"

# 4. Validación de longitud (criterio 3)
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":91,"granArea":"Un texto de mas de sesenta caracteres para forzar el error de longitud maxima","area":"x","disciplina":"x"}'
# esperado: 400, detalle de validación sobre GranArea

# 5. Editar y confirmar (criterio 4)
curl.exe -X PUT http://localhost:8080/api/area_conocimiento/90 `
  -H "Content-Type: application/json" `
  -d '{"granArea":"Prueba editada","area":"Prueba","disciplina":"Prueba"}'
curl.exe http://localhost:8080/api/area_conocimiento/90
# esperado: el segundo curl muestra "granArea":"Prueba editada"

# 6. Eliminar (criterio 5)
curl.exe -X DELETE http://localhost:8080/api/area_conocimiento/90
curl.exe http://localhost:8080/api/area_conocimiento/90
# esperado: DELETE -> 204 ; GET posterior -> 404
```

Verificación del borrado lógico por SQL directo (SQLTools, conexión a
`localhost:15432`, credenciales del Artículo 8):
```sql
SELECT id, activo FROM area_conocimiento WHERE id = 90;
-- debe devolver una fila con activo = false, no cero filas
```

**7. El mismo ciclo desde el frontend (criterio 6):** repetir los pasos
2, 5 y 6 en `http://localhost:8081/AreaConocimiento` usando "+ Nuevo
registro", "Editar" y "Eliminar" en vez de `curl`.

## Cierre de la entrega

Se da por cerrada solo cuando los 7 pasos de arriba pasan sin fallos
distintos a los esperados explícitamente (Artículo 1: commit + subida al
repositorio, solo entonces se abre la spec de la Entrega 2).
