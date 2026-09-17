# 4_research.md — Versión 1: catálogos sin llave foránea

Decisiones tomadas para esta entrega, numeradas, con las alternativas que
se consideraron y por qué se descartaron. Sirve para no volver a discutir
lo ya decidido en entregas futuras.

## D1 — El id se recibe en la petición, no se genera automáticamente

**Decisión:** las 22 tablas propias del módulo reciben su llave primaria
como parte del `POST`, y la API valida que no exista antes de insertar.

**Alternativa descartada:** modificar el script SQL para que las llaves
primarias fueran `SERIAL` (autoincremento).

**Por qué se descartó:** el artículo 5 de la constitución prohíbe
modificar el script entregado por el profesor. Además, la carga futura
de datos de referencia (por ejemplo, las 218 filas de `area_conocimiento`
del Excel del profesor) probablemente ya trae ids específicos que deben
respetarse tal cual, no reasignarse.

## D2 — Se agrega la columna `activo` con un script aparte

**Decisión:** `db/01_alter_activo.sql` agrega `activo BOOLEAN DEFAULT TRUE`
a las 22 tablas del módulo (solo `usuario` y `rol` ya la traían en el
script original).

**Alternativa descartada:** editar directamente el `CREATE TABLE` del
script original para incluir la columna desde el inicio.

**Por qué se descartó:** el artículo 5 exige que el script dado no se
toque; un script adicional deja explícito y auditable qué se modificó y
por qué, sin perder la trazabilidad de qué vino del profesor y qué
agregó el equipo.

## D3 — Dapper en vez de Entity Framework

**Decisión:** todo el acceso a datos se escribe con Dapper y SQL manual
parametrizado.

**Alternativa descartada:** Entity Framework Core con migraciones
automáticas.

**Por qué se descartó:** el artículo 3 de la constitución lo prohíbe
explícitamente — EF generaría su propio esquema o requeriría mapear un
esquema ya existente de forma más compleja que escribir el SQL a mano
para 7 tablas simples; además, Dapper deja ver exactamente qué consulta
se ejecuta, más fácil de auditar contra `6_contracts.md`.

## D4 — Arquitectura en 4 capas, repetida idéntica en las 7 tablas

**Decisión:** Modelo → Petición → Repositorio → Servicio → Controller,
mismo patrón sin variación entre tablas.

**Alternativa descartada:** un único Controller genérico que reciba el
nombre de la tabla por parámetro y arme el SQL dinámicamente.

**Por qué se descartó:** un Controller genérico rompería el artículo 3
(SQL parametrizado por campo, no ensamblado dinámicamente) y perdería la
validación específica de cada tabla (los `[MaxLength]` distintos de cada
campo). Repetir el patrón es más código, pero cada archivo es trivial de
leer y de corregir de forma aislada.

## D5 — Frontend en Razor Pages, no en una SPA aparte

**Decisión:** el frontend se construye en ASP.NET Core Razor Pages, mismo
lenguaje que el backend.

**Alternativa descartada:** React, Angular o Vue como aplicación separada.

**Por qué se descartó:** el PDS del módulo permite explícitamente
tecnologías "React/Angular/Vue/HTML-JS" en el frontend; Razor Pages
genera HTML/JS del lado del servidor y cumple esa restricción, con la
ventaja de no introducir un segundo lenguaje/ecosistema (Node, npm) bajo
el tiempo disponible para esta entrega. Queda como decisión abierta a
revisar en una entrega futura si el equipo prefiere una SPA.

## D6 — `aliado` usa `nit` como llave primaria, no `id`

**Decisión:** en las capas de `aliado`, todas las rutas y parámetros usan
`nit` en vez de `id`.

**Alternativa descartada:** normalizar el nombre a `id` en la API aunque
la columna real se llame `nit`.

**Por qué se descartó:** el script de base de datos define la columna
como `nit`; renombrarla en la API crearía una inconsistencia entre lo que
dice el contrato y lo que hay en la base de datos, dificultando que
alguien nuevo entienda el sistema leyendo solo la API.

## D7 — Repositorios de GitHub independientes para API y frontend

**Decisión:** `api-innovacion-curricular` y `frontend-innovacion-curricular`
son dos repositorios separados, cada uno con sus propias ramas por
integrante.

**Alternativa descartada:** un solo repositorio monolítico con ambos
proyectos adentro.

**Por qué se descartó:** lo exige explícitamente el PDS del módulo
("repositorio independiente para el Backend y otro para el Frontend"),
además de reforzar en la práctica la separación de responsabilidades del
artículo 1.
