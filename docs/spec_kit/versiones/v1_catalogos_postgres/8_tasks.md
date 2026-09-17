# 8_tasks.md — Entrega 1: catálogos sin llave foránea

Cada fase cubre las 7 tablas a la vez. No se avanza con la verificación
de la fase actual en rojo (Artículo 1).

```mermaid
flowchart LR
    F0["Fase 0<br/>Base de datos"] --> F1["Fase 1<br/>Modelos"]
    F1 --> F2["Fase 2<br/>Peticiones/Excepciones"]
    F2 --> F3["Fase 3<br/>Repositorios"]
    F3 --> F4["Fase 4<br/>Servicios"]
    F4 --> F5["Fase 5<br/>Controllers"]
    F5 --> F6["Fase 6<br/>Docker API"]
    F6 --> F7["Fase 7<br/>Frontend"]
    F7 --> F8["Fase 8<br/>Cierre"]
```

| Fase | Tareas | Verificar |
|---|---|---|
| **0** — Base de datos | Copiar `00_...sql` del profesor · escribir `01_alter_activo.sql` · servicio `db` en `docker-compose.yml` | `docker exec -it innovacion_db psql -U postgres -d innovacion_curricular -c "\d area_conocimiento"` muestra la columna `activo` |
| **1** — Modelos | `.csproj` con Dapper/Npgsql/Swashbuckle · 7 clases en `Modelos/` | `dotnet build` sin errores |
| **2** — Peticiones/Excepciones | 7 `{Tabla}Peticiones.cs` con `[Required]`/`[MaxLength]` · `ExcepcionesNegocio.cs` | `dotnet build` sin errores |
| **3** — Repositorios | 7 interfaces + implementaciones Dapper, filtrando `activo = true` en lecturas | revisión manual: todo SQL usa `@parametros` (Artículo 2) |
| **4** — Servicios | 7 interfaces + implementaciones: id no repetido (D1), `NoEncontradoExcepcion` | `dotnet build` sin errores |
| **5** — Controllers | 7 Controllers con las rutas de `6_contracts.md` · `Program.cs` registra las 7 parejas Repositorio/Servicio | `dotnet build` sin errores dentro de `api_innovacion/` |
| **6** — Docker API | `Dockerfile` · servicio `api` en `docker-compose.yml` | `curl.exe http://localhost:8080/api/area_conocimiento` → `200` |
| **7** — Frontend | `.csproj` + `Program.cs` con `HttpClient` → `API_URL` · 7 Modelos espejo · 7 clientes HTTP · páginas Index/Crear/Editar ×7 · `_Layout.cshtml` · `Dockerfile` · servicio `frontend` | `http://localhost:8081/AreaConocimiento` carga sin error |
| **8** — Cierre | Correr completo `7_quickstart.md` · subir a los dos repos de GitHub, cada uno con rama por integrante (Artículo 1 del PDS) | los 6 criterios de `2_spec.md` en verde; ambos repos con `main` + una rama por integrante |

## Estado real de esta entrega (al escribir este documento)

Las fases 0 a 7 están completas y verificadas manualmente (Swagger y
frontend probados en vivo). La fase 8 —smoke test formal con los
comandos exactos de `7_quickstart.md`, y confirmación de que ambos repos
tienen rama por integrante— queda para correr como cierre antes de la
sustentación.
