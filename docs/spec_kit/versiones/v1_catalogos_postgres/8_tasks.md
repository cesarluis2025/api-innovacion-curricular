# 8_tasks.md — Versión 1: catálogos sin llave foránea

Cada fase cubre las 7 tablas a la vez (a diferencia del ejemplo de
facturas, que construye una sola entidad por versión). No se avanza a la
fase siguiente con la verificación de la actual en rojo.

## Fase 0 — Base de datos
- T1. Copiar `db/00_innovacion_curricular.pg.sql` tal cual lo entregó el
  profesor (artículo 5, no se modifica).
- T2. Escribir `db/01_alter_activo.sql` con el `ALTER TABLE ... ADD
  COLUMN activo` de las 22 tablas del módulo (decisión D2).
- T3. Escribir `docker-compose.yml` con el servicio `db` (PostgreSQL 16),
  montando `./db` en `docker-entrypoint-initdb.d`.

**Verificar:**
```powershell
docker compose up -d db
# esperar unos segundos
docker exec -it innovacion_db psql -U postgres -d innovacion_curricular -c "\d area_conocimiento"
```
Debe listar la columna `activo` al final de la tabla.

## Fase 1 — Modelos
- T4. `api_innovacion/ApiInnovacionCurricular.csproj` con los paquetes
  Dapper, Npgsql, Swashbuckle.AspNetCore.
- T5. Las 7 clases en `Modelos/` (una por tabla, artículo 7).

**Verificar:** el proyecto compila sin las demás capas todavía (los
Modelos no dependen de nada más): `dotnet build` dentro de
`api_innovacion/` no debe fallar por los archivos de `Modelos/`.

## Fase 2 — Peticiones y excepciones
- T6. Las 7 clases `{Tabla}Peticiones.cs` con `[Required]`/`[MaxLength]`
  según `5_data_model.md`.
- T7. `Excepciones/ExcepcionesNegocio.cs` con `NoEncontradoExcepcion` y
  `ConflictoExcepcion` (compartidas por las 7 tablas).

**Verificar:** `dotnet build` sigue sin errores.

## Fase 3 — Repositorios
- T8. Las 7 interfaces + implementaciones en `Repositorios/`, con las
  consultas Dapper (`ListarAsync`, `ObtenerPorIdAsync`, `ExisteIdAsync`,
  `CrearAsync`, `ActualizarAsync`, `EliminarLogicoAsync`), filtrando
  siempre por `activo = true` en las lecturas.

**Verificar:** cada consulta usa `@parametros` (artículo 3) — revisión
manual de que ningún string se concatena en el SQL.

## Fase 4 — Servicios
- T9. Las 7 interfaces + implementaciones en `Servicios/`, con la regla
  de "id no repetido" al crear (D1) y el lanzamiento de
  `NoEncontradoExcepcion` cuando corresponde.

**Verificar:** `dotnet build` sigue sin errores.

## Fase 5 — Controllers y ensamblado
- T10. Los 7 `Controller` en `Controllers/`, exponiendo las rutas de
  `6_contracts.md` y traduciendo excepciones a 400/404.
- T11. `Program.cs`: registra los 7 pares Repositorio/Servicio en el
  contenedor de dependencias, prende Swagger.

**Verificar:**
```powershell
cd api_innovacion
dotnet build
```
Sin errores de compilación.

## Fase 6 — Dockerización de la API
- T12. `api_innovacion/Dockerfile`.
- T13. Servicio `api` en `docker-compose.yml`, con `CONNECTION_STRING`
  apuntando al servicio `db`.

**Verificar:**
```powershell
docker compose up -d --build
curl.exe http://localhost:8080/api/area_conocimiento
```
Responde `200` con un arreglo JSON (vacío o con datos).

## Fase 7 — Frontend
- T14. `frontend_innovacion/FrontendInnovacionCurricular.csproj`,
  `Program.cs` con el `HttpClient` apuntando a `API_URL`.
- T15. Las 7 clases `Modelos/` (espejo de la API).
- T16. Los 7 "clientes HTTP" en `Servicios/` (uno por tabla).
- T17. Las páginas Razor (`Index`, `Crear`, `Editar`) para cada una de
  las 7 tablas, más `Pages/Shared/_Layout.cshtml` con el menú de
  navegación.
- T18. `frontend_innovacion/Dockerfile` y el servicio `frontend` en
  `docker-compose.yml`.

**Verificar:**
```powershell
docker compose up -d --build
```
Abrir `http://localhost:8081/AreaConocimiento` en el navegador: la tabla
carga sin error.

## Fase 8 — Cierre de la versión
- T19. Correr completo el smoke test de `7_quickstart.md` (los 7 pasos).
- T20. Subir el código a los dos repositorios de GitHub
  (`api-innovacion-curricular`, `frontend-innovacion-curricular`), cada
  uno con su rama por integrante (artículo 11).

**Verificar:** los 6 criterios de aceptación de `2_spec.md` en verde, y
ambos repositorios muestran `main` + una rama por cada integrante del
equipo.

---

## Estado real de esta versión (al escribir este documento)

Todas las tareas T1 a T18 están completas y verificadas manualmente
(Swagger y frontend probados en vivo). La T19 (smoke test formal con los
comandos exactos de arriba) y la T20 (ambos repos con ramas de los dos
integrantes) quedan para correr como cierre antes de la sustentación.
