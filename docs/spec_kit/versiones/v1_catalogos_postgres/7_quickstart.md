# 7_quickstart.md — Versión 1: catálogos sin llave foránea

## Arranque

Parado en la raíz del repositorio (donde está `docker-compose.yml`):

```powershell
docker compose up -d --build
```

Espera 1-2 minutos la primera vez (descarga PostgreSQL, corre los scripts
de `db/`, compila la API y el frontend). Confirma con `docker ps` que
`innovacion_db`, `innovacion_api` e `innovacion_frontend` estén corriendo.

- API: `http://localhost:8080` (Swagger en `http://localhost:8080/swagger`)
- Frontend: `http://localhost:8081`

## Smoke test — recorre los 6 criterios de aceptación de `2_spec.md`

Ejecutar en PowerShell, en orden. Se usa `area_conocimiento` como tabla de
prueba; el mismo patrón aplica a las otras 6.

**1. Listar (criterio 1) — debe responder 200 con un arreglo, aunque esté vacío**
```powershell
curl.exe http://localhost:8080/api/area_conocimiento
```

**2. Crear (para preparar los siguientes pasos)**
```powershell
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":90,"granArea":"Prueba","area":"Prueba","disciplina":"Prueba"}'
```
Esperado: **201**, el objeto creado con `"activo":true`.

**3. Duplicado (criterio 2) — repetir el mismo id**
```powershell
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":90,"granArea":"Otra","area":"Otra","disciplina":"Otra"}'
```
Esperado: **400** con `"mensaje": "ya existe un área de conocimiento con id 90"`.

**4. Validación de longitud (criterio 3) — un campo demasiado largo**
```powershell
curl.exe -X POST http://localhost:8080/api/area_conocimiento `
  -H "Content-Type: application/json" `
  -d '{"id":91,"granArea":"Un texto de mas de sesenta caracteres para forzar el error de longitud maxima","area":"x","disciplina":"x"}'
```
Esperado: **400** con el detalle de validación sobre `GranArea`.

**5. Ciclo completo (criterio 4) — editar y confirmar el cambio**
```powershell
curl.exe -X PUT http://localhost:8080/api/area_conocimiento/90 `
  -H "Content-Type: application/json" `
  -d '{"granArea":"Prueba editada","area":"Prueba","disciplina":"Prueba"}'
curl.exe http://localhost:8080/api/area_conocimiento/90
```
Esperado: el segundo `curl` muestra `"granArea":"Prueba editada"`.

**6. Eliminar y verificar borrado lógico (criterio 5) — sigue en la BD, pero inactivo**
```powershell
curl.exe -X DELETE http://localhost:8080/api/area_conocimiento/90
curl.exe http://localhost:8080/api/area_conocimiento/90
```
Esperado: el `DELETE` responde **204**; el `GET` posterior responde
**404** (porque el listado/consulta filtra por `activo = true`), pero el
registro sigue en la base de datos. Para comprobarlo de verdad, con
SQLTools (conexión a `localhost:15432`, ver `README.md`):
```sql
SELECT id, activo FROM area_conocimiento WHERE id = 90;
-- debe devolver una fila con activo = false, no cero filas
```

**7. El mismo ciclo desde el frontend (criterio 6)**
Repetir los pasos 2, 5 y 6 pero usando `http://localhost:8081/AreaConocimiento`
en el navegador en vez de `curl`: crear con "+ Nuevo registro", confirmar
que aparece en la tabla, editarlo, y eliminarlo confirmando que
desaparece del listado.

## Cierre de la versión

La versión 1 se da por cerrada solo cuando los 7 pasos de arriba (que
cubren los 6 criterios de `2_spec.md`) se ejecutan sin fallos distintos a
los esperados explícitamente.
