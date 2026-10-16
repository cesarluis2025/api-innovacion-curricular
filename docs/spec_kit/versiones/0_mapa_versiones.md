# Mapa de versiones — Módulo de innovación curricular

> La ruta completa del proyecto. Cada versión se especifica **solo cuando la
> anterior está cerrada** (commit + tag). Este mapa da la dirección; el spec
> kit de cada versión da el detalle.
>
> **Y cada versión entrega su API Y SU INTERFAZ GRÁFICA.** No hay una versión
> «de back» y otra «de front»: se construyen en paralelo, y una versión no está
> cerrada si la API responde y la interfaz gráfica no.
>
> La ruta es la que define `0_METODOLOGIA.md` §2; aquí no se inventa nada, se
> ordena. El detalle de la v2 está en `VERSION_2.md`.

## La ruta — **cuatro versiones**

> **Cada versión INCLUYE la anterior.** La v2 incluye la v1, la v3 incluye la
> v2 y la v4 incluye la v3. No se reinicia nada: lo construido sigue en pie,
> con su código, su interfaz gráfica y sus criterios de aceptación —y esos
> criterios se vuelven a correr, que es lo que se llama **la regresión**.

| Versión | Qué agrega (acumulativo) | Estado |
|---|---|---|
| **v1** | CRUD completo de **las 7 tablas sin clave foránea** — **API e interfaces gráficas** | **Cerrada**, tag `v1` ([spec](v1_catalogos_postgres/2_spec.md)) |
| **v2** | CRUD de **las 15 tablas con clave foránea** — con la v2 están las **22 tablas propias del módulo**: las FK como **listas desplegables cargadas desde la API**, las tablas puente, el maestro-detalle, **todo el CRUD por procedimientos almacenados** y al menos un disparador — **API e interfaces gráficas** | **En curso** ([spec](v2_tablas_con_fk/2_spec.md)) |
| v3 | **El control de acceso**: JWT, sesión, permisos por rol, y el CRUD de `usuario`, `rol` y `rol_usuario` (solo administrador) | Pendiente |
| v4 | **10 consultas multitabla** (4+ tablas cada una), tablero con gráficos, **imagen corporativa con su manual de marca**, páginas corporativas, responsive/PWA y **publicación** en un servidor | Pendiente |

## El reparto de las 25 tablas

La base `innovacion_curricular` tiene 25 tablas: 22 del módulo y 3 de gestión
de usuarios. Repartidas:

| Versión | Tablas | Criterio |
|---|---|---|
| v1 | `area_conocimiento` · `universidad` · `aspecto_normativo` · `practica_estrategia` · `enfoque` · `car_innovacion` · `aliado` | **Las 7 sin clave foránea.** Se pueden llenar sin que exista nada más |
| v2 | `facultad` · `programa` · `acreditacion` · `registro_calificado` · `activ_academica` · `pasantia` · `premio` · `programa_ac` · `programa_pe` · `programa_ci` · `an_programa` · `enfoque_rc` · `aa_rc` · `docente_departamento` · `alianza` | **Las 15 con clave foránea**, incluidas las 8 puente o compuestas. Con la v2, las **22** del módulo están. Además, las 7 de la v1 pasan a procedimientos almacenados |
| v3 | `usuario` · `rol` · `rol_usuario` — y **la puerta**: `login`, sesión con token, permisos | Las 3 de control de acceso. `VERSION_2.md` §0 las deja explícitamente fuera de la v2 |
| v4 | `consultas` — **y no es una tabla** | **No agrega tablas.** Agrega las consultas que CRUZAN las 25: diez preguntas que ninguna tabla responde sola |

> **Ojo:** las 25 tablas **existen en la base desde la v1** (el script del
> profesor las trae todas). Lo que reparte esta tabla es qué puede **nombrar el
> código** de cada versión, no qué existe en el motor.

> **`docente` no es una tabla de este módulo.** `docente_departamento.docente`
> y `alianza.docente` son `INT` **sin** clave foránea: el docente vive en
> otro módulo. Por eso no se valida contra nada.

## Las relaciones maestro-detalle (v2)

| Maestro | Su detalle |
|---|---|
| `universidad` | `facultad` |
| `facultad` | `programa` |
| `programa` | `acreditacion` · `activ_academica` · `pasantia` · `premio` · `registro_calificado` |

## La estrategia: back y front EN PARALELO

**Cada versión entrega su parte de la API *y* su parte del front.**

| | |
|---|---|
| **Lo terminado se le puede mostrar a alguien** | Swagger demuestra que la API responde; la interfaz gráfica demuestra que el sistema sirve |
| **El contrato se ejercita de inmediato** | Pintar un JSON es la mejor forma de descubrir que está mal pensado |
| **No hay front de golpe al final** | Que es el error que se paga caro |
| **Es lo que pide el curso** | `0_METODOLOGIA.md` §2: *«CRUD de las tablas sin FK del módulo — API REST + Frontend funcionando»* |

> **La regla operativa:** una versión **no está cerrada** si la API responde y
> la interfaz gráfica no.

## Historial de cambios

| Fecha | Cambio |
|---|---|
| 2026-10-15 | Mapa creado (la v1 se cerró sin él). Se reescribe tomando como molde el mapa del ejemplo del profesor |
