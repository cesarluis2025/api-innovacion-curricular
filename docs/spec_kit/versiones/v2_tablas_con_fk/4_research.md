# 4_research.md — Versión 2: tablas con llave foránea, procedimientos y disparador

Decisiones tomadas para esta versión, cada una con su alternativa descartada,
para no reabrir la discusión después. Las de la v1 (D1–D7) siguen vigentes
salvo donde esta versión las enmienda (se indica). Numeración propia: `D-v2-N`.

---

## D-v2-1 — Los `sp_*` son FUNCIONES de PostgreSQL, no `PROCEDURE`
**Decisión:** `CREATE FUNCTION sp_crear_programa(...) RETURNS programa` y las
demás, con `RETURNING *` y `RETURNS SETOF` / `RETURNS TABLE`.
**Descartada:** `CREATE PROCEDURE` invocado con `CALL`.
**Por qué:** un `PROCEDURE` no devuelve filas (solo parámetros `OUT`), y la v2
exige que crear y actualizar devuelvan **la fila como quedó guardada**
(`VERSION_2.md` §4) y que listar devuelva un conjunto. Los ejemplos del
profesor también son funciones. «Procedimiento almacenado» se usa en el curso
en sentido general: lógica con nombre guardada en la base.

## D-v2-2 — Se llama con `SELECT * FROM sp_xxx(@p)`, no con `CommandType.StoredProcedure`
**Decisión:** cada método del repositorio ejecuta **una** sentencia, la llamada
`SELECT * FROM sp_xxx(@p1, @p2)` con `CommandType.Text` y parámetros.
**Descartadas:**
- `CommandType.StoredProcedure` sin más: desde Npgsql 7.0 genera SQL para
  invocar un *procedimiento* (`CALL`), «y no una función, como antes», según
  las [notas de la versión 7.0](https://www.npgsql.org/doc/release-notes/7.0.html);
  fallaría contra nuestras funciones.
- `CommandType.StoredProcedure` + `AppContext.SetSwitch("Npgsql.EnableStoredProcedureCompatMode", true)`:
  funciona, pero es un interruptor **global y de compatibilidad**: si Npgsql lo
  retira, se rompe todo el acceso a datos de un golpe.

| Opción | Funciona con funciones | Riesgo | SQL en el repo |
|---|---|---|---|
| `SELECT * FROM sp_xxx(...)` (texto) | sí | ninguno: es lo que recomienda Npgsql | solo la llamada |
| `StoredProcedure` + modo compat | sí | depende de un interruptor de transición | ninguno |
| `StoredProcedure` sin más | **no** (genera `CALL`) | rompe | — |

**Por qué:** es la forma que la propia documentación de Npgsql da para
funciones. La regla de la v2 («no hay SQL escrito a mano en el repositorio»)
se entiende como **ninguna lógica de tablas**: la llamada es una invocación con
nombre, no una consulta.
**Consecuencia:** la prueba del criterio 3 busca `SELECT`/`INSERT`/`UPDATE`/`DELETE`
que **no** sean `FROM sp_` (`2_spec.md` y constitución Art. 2 ya lo dicen).
Esta decisión **corrigió** la primera versión de `3_plan.md`, que decía
`CommandType.StoredProcedure`.

## D-v2-3 — Los errores de la base se traducen en UN middleware; la base es la autoridad
**Decisión:** `ManejadorErrores` atrapa `PostgresException` y responde por
`SqlState`: `23503` (llave foránea) y `23505` (llave repetida) → **409**; los
demás → 500. El mensaje se arma en castellano con `TableName` y
`ConstraintName`, **sin leer el `Detail`** (ese texto sale en el idioma del
servidor, inglés por defecto).
**Descartada:** validar todo en el servicio con consultas previas
(«¿existe el padre?») y dejar la base como respaldo mudo.
**Por qué:** una validación previa tiene una carrera: entre «existe» y el
`INSERT` alguien puede retirar el padre. La restricción de la base es la única
que no se puede saltar. **El servicio sigue haciendo la comprobación previa de
llave repetida** donde es barata, solo para dar el mensaje amable de la v1
(«ya existe un … con id N»); si otra petición llega primero, el middleware
responde el 409 genérico.
**Consecuencia:** un 500 donde correspondía un 409 es criterio fallado
(criterio 7), y por eso la colección de pruebas incluye las peticiones que
deben fallar.

## D-v2-4 — La regla de retiro (B1) vive en el servicio
**Decisión:** antes de retirar un maestro (`universidad`, `facultad`, `programa`)
el servicio llama a `TieneDetalleActivoAsync` (que llama a un `sp_*` de
conteo) y lanza `ConflictoExcepcion` → 409.
**Descartada:** hacer que `sp_eliminar_*` lance un error (`RAISE EXCEPTION`)
cuando haya detalle activo.
**Por qué:** `VERSION_2.md` §7 pide que la regla de negocio se ponga «en el
servicio» y se escriba en la spec. **Lo que se pierde:** quien retire con
`psql` por fuera de la API no encuentra la regla. Se acepta: la v2 protege el
camino de la aplicación, no el administrador de la base.
**Alcance:** los tres maestros del enunciado; el resto de padres solo queda
protegido contra el borrado físico por su llave foránea.

## D-v2-5 — Varios renglones de detalle viajan en UN envío: arreglo JSON a una función
**Decisión:** `sp_crear_<detalle>_lote(p_maestro INT, p_renglones JSONB)` abre
el arreglo con `jsonb_to_recordset` e inserta todos. **Una función = una
transacción:** si falla un renglón, no se guarda ninguno.
**Descartadas:**
- Una petición por renglón: tres renglones serían tres peticiones y, si la
  tercera fallara, quedaría «medio registro» (`VERSION_2.md` §3).
- Abrir la transacción en C# y llamar N veces a `sp_crear_*`: es correcto,
  pero reparte la integridad entre dos capas y cuesta N viajes a la base.
**Por qué:** el comentario que el profesor pone como ejemplo bueno lo dice:
«UN viaje a la base y UNA transacción».
**Consecuencia:** el error del lote nombra el renglón que falló; el contrato
exacto va en `6_contracts.md`.

## D-v2-6 — Disparador: `facultad.total_programas`, recalculado con `COUNT`, para ambos lados
**Decisión:** (propuesta A) columna `total_programas INT DEFAULT 0` en
`facultad` y un disparador `AFTER INSERT OR UPDATE OR DELETE ON programa` que
**recalcula con `COUNT(*) … WHERE activo`**, no suma ni resta uno.
**Descartadas:** incrementar y decrementar (se desincroniza al primer fallo);
dejar el contador a cargo de la API (`VERSION_2.md` §5: «si su código manda el
total, el disparador no está demostrando nada»).
**Corrección sobre el ejemplo del profesor:** su propuesta A usa
`COALESCE(NEW.x, OLD.x)`, que en un `UPDATE` solo mira el valor **nuevo**. Si un
programa **cambia de facultad**, la facultad antigua queda con el contador
desfasado. Nuestro disparador también recalcula `OLD.facultad` cuando
`OLD.facultad IS DISTINCT FROM NEW.facultad`.
**Escucha el `UPDATE`** porque retirar es `UPDATE … SET activo = false`
(trampa 1 de `VERSION_2.md` §12).
**Orden de scripts:** el disparador (`05`) se crea **antes** de cargar los datos
(`06`), así los programas del Excel se cuentan solos al insertarse.

## D-v2-7 — Tablas puente: «retirar» es lógico y repetir una pareja retirada la reactiva (B6)
**Decisión:** `sp_crear_<puente>` primero intenta reactivar la pareja si existe
con `activo = false`; si no existe, la inserta. Si existe **activa**, el
`INSERT` viola la llave primaria → `23505` → 409.
**Descartada:** dar 409 también cuando la pareja está retirada.
**Por qué:** el usuario ve la pareja como inexistente (el listado filtra
inactivos); responderle «ya existe» sería contradecir lo que ve. Se mantiene el
nombre `sp_crear_*` (convención de `VERSION_2.md` §4); en pantalla el verbo es
«asignar».
**Sin «editar»:** una pareja existe o no existe (trampa 6). Solo `aa_rc`,
`docente_departamento` y `alianza`, que tienen campos propios, se actualizan.

## D-v2-8 — Los desplegables los llena el servidor del frontend, no el navegador
**Decisión:** el `PageModel` de cada formulario pide al cliente HTTP la lista
del padre (`GET /api/{padre}`, solo activos) y arma el `<select>`: texto =
nombre legible, `value` = llave. Una FK opcional lleva «(ninguna)» y el
`PageModel` convierte la cadena vacía en `null` (`int?`).
**Descartada:** `fetch` desde JavaScript del navegador hacia la API.
**Por qué:** la API vive en la red de Docker; llamarla desde el navegador
obliga a exponer su dirección al cliente y a configurar CORS, cosas que la v2
no necesita. Además, una cadena vacía donde va un número produce un error de
conversión en inglés (`VERSION_2.md` §6).
**Texto que se muestra por tabla** (se fija en `5_data_model.md`): p. ej.
`universidad.nombre`, `facultad.nombre`, `programa.nombre`.

## D-v2-9 — Maestro-detalle: una pantalla de detalle por maestro, con renglones agregables
**Decisión:** `Pages/Universidad`, `Pages/Facultad` y `Pages/Programa` llevan
una página `Detalle` que muestra el detalle del maestro y permite agregar
varios renglones antes de enviar (un solo `POST` al lote, D-v2-5).
**Descartada:** un menú aparte para cada tabla hija donde haya que volver a
elegir el maestro (lo rechaza `VERSION_2.md` §3).
**Por qué:** «un detalle no existe sin su maestro»: la pantalla lo fija de
entrada. Las 15 tablas siguen teniendo además su CRUD plano propio.

## D-v2-10 — Los secretos salen del repositorio; la contraseña se cambia (rotación)
**Decisión:** `docker-compose.yml` lee `${DB_PASSWORD}` (y la cadena de
conexión de la API); `.env` va en `.gitignore`; `.env.example` trae valores de
mentira.
**Descartada:** dejar la contraseña «didáctica» de la v1.
**Por qué:** la metodología (§6) aclara que la excepción didáctica es de los
repos del profesor; en el proyecto de aula la regla aplica desde la v1, y un
secreto en el código **anula el criterio de seguridad**. Como la contraseña
vieja quedó en el historial de git, borrarla del último commit no basta: hay
que **cambiarla**.
**Consecuencia práctica:** PostgreSQL fija la contraseña al **crear** el
volumen. Para que la nueva tenga efecto hay que recrear la base:
`docker compose down -v` y `docker compose up -d --build`. Se pierden los
datos de prueba de la v1 (que el script `06` vuelve a cargar).

## D-v2-11 — La v1 se migra a `sp_*` sin cambiar su contrato, salvo el 409
**Decisión:** las 7 tablas de la v1 pasan a funciones (`db/03_sp_v1.sql`) con
las mismas rutas y los mismos cuerpos. Lo único que cambia es el código de
«llave repetida»: 400 → 409 (enmienda de la constitución, Art. 3 y 8).
**Descartada:** dejar las 7 tablas de la v1 con SQL en el repositorio y solo
las nuevas con procedimientos.
**Por qué:** el criterio 3 de la v2 exige **los cinco verbos de todas las
tablas**, y la regresión (criterio 12) corre los criterios de la v1.
**Consecuencia:** el criterio 2 de la v1 («duplicado → 400») se evalúa en la
regresión como 409; la v1 no se reabre ni se reescribe su spec (regla de oro 4
de la metodología).

## D-v2-12 — Los datos de referencia salen de `Base de Datos v6.xlsx` con un script que corrige lo que el Excel y la base no comparten
**Fuente:** `Mapa_conocimiento/04_Modelo_y_Base_de_datos/Base de Datos v6.xlsx`,
hojas `area_conocimiento` (218), `universidad` (6), `facultad` (35) y
`programa` (191): los mismos conteos del documento del módulo. El encabezado de
cada hoja está en la fila 3 y la columna A va vacía.
**Decisión:** `db/06_datos_referencia.sql`, en orden de dependencia
(`universidad`, `area_conocimiento`, `facultad`, `programa`), con estas reglas:
1. **Ids de área:** en el Excel son códigos (`1A01`…`6E03`) y la columna es
   `INT`. Se numeran **1 a 218 en el orden del Excel**; el código original queda
   en un comentario junto a cada fila.
2. **Textos largos:** 19 `disciplina` (hasta 124 caracteres) y 25
   `programa.nombre` (hasta 92) no caben en `VARCHAR(60)`. `db/02_alter_v2.sql`
   **amplía** `area_conocimiento.disciplina` a `VARCHAR(130)` y `programa.nombre`
   a `VARCHAR(100)`. Se actualiza el límite en la validación de esas dos tablas.
3. **Columnas obligatorias que el Excel no trae** (`facultad.fecha_fun`;
   `programa`: `nivel`, `fecha_creacion`, `numero_cohortes`,
   `cant_graduados`, `fecha_actualizacion`, `ciudad`): valor de relleno
   documentado en el propio script, `'Sin dato'` para texto y `1900-01-01` para
   la fecha; se corrigen luego desde la interfaz.
4. **Ortografía:** «Cienias Naturales» (48 filas) se carga como «Ciencias
   Naturales», anotado en el script. El resto se carga tal cual, incluido
   `facultad.tipo = 'Unidad'`.
**Descartadas:** cortar a 60 caracteres (deja 44 nombres sin sentido); quitar
`NOT NULL` a las columnas faltantes (cambia más el esquema del profesor que
ampliar dos longitudes); cargar a mano por la interfaz (los desplegables
saldrían vacíos, `VERSION_2.md` §1).
**Por qué ampliar y no cortar:** el Artículo 5 admite lo que el script dado no
trae, en scripts numerados aparte, y perder el nombre de un programa es perder
datos reales. **Lo que cuesta:** el criterio de longitud de la v1 («campo que
excede su máximo → 400») se corre en la regresión con los límites nuevos
(disciplina 130, programa.nombre 100).

## D-v2-13 — La llave foránea NO sabe de `activo`: cada `sp_crear_*`/`sp_actualizar_*` hijo valida que el padre esté activo
**Hallazgo (al leer `00_innovacion_curricular.pg.sql`):** las llaves foráneas
del script solo comprueban que la fila padre **exista**; un padre retirado
(`activo = false`) sigue existiendo, así que la base aceptaría un hijo nuevo
colgado de él, y `2_spec.md` (RF4, RF9) lo prohíbe.
**Decisión:** dentro de cada función que escribe un hijo, antes del `INSERT` o
`UPDATE`, se comprueba `EXISTS (SELECT 1 FROM padre WHERE id = p AND activo)`;
si no, `RAISE EXCEPTION ... USING ERRCODE = '23503', TABLE = '<padre>', COLUMN = '<campo>'`.
**Descartada:** confiar solo en la llave foránea, o validar en el servicio con
una consulta previa (misma carrera de D-v2-3).
**Por qué:** usar el **mismo** código `23503` que lanza una FK real hace que el
middleware (D-v2-3) responda 409 por un único camino, tanto si el padre no
existe como si está retirado. `TABLE` y `COLUMN` dejan armar el mensaje en
castellano sin leer texto en inglés. Refina D-v2-3: para una FK nativa el campo
se deduce del nombre de la restricción (`{tabla}_{columna}_fkey`, ver
`5_data_model.md`).

## D-v2-14 — `activ_academica`: `espejo` es 0/1 y, si es 0, los dos campos espejo llevan «No aplica»
**Hallazgo:** en el script, `espejo SMALLINT`, `entidad_espejo` y `pais_espejo`
son **todos `NOT NULL`**, aunque una actividad que no es espejo no tiene
entidad ni país.
**Decisión:** `espejo` se valida 0 o 1 (casilla en la pantalla). Si es 0, el
formulario deshabilita los dos campos y el servicio guarda el texto
`No aplica`. Si es 1, ambos son obligatorios.
**Descartada:** volver los campos `NULL` (el Art. 5 prohíbe editar el script
dado) o dejar que la persona escriba cualquier cosa.
**Por qué:** respeta la base tal como la entregó el profesor y evita que la
pantalla pida un dato que no existe.
