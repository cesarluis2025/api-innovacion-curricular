# 4_research.md — Entrega 1: catálogos sin llave foránea

Decisiones tomadas para esta entrega. Cada una con su alternativa
descartada — para no reabrir la discusión en entregas futuras.

## D1 — El id se recibe en la petición, no se genera automáticamente
**Decisión:** las 22 tablas propias del módulo reciben su llave primaria
en el `POST`; la API valida que no exista antes de insertar.
**Descartada:** convertir las llaves a `SERIAL`.
**Por qué:** el Artículo 5 prohíbe modificar el script entregado; además,
la carga futura de datos de referencia ya trae ids específicos que deben
respetarse.

## D2 — La columna `activo` se agrega con un script aparte
**Decisión:** `db/01_alter_activo.sql` agrega `activo BOOLEAN DEFAULT
TRUE` a las 22 tablas del módulo.
**Descartada:** editar el `CREATE TABLE` original.
**Por qué:** el Artículo 5 exige que el script dado no se toque; un
script numerado aparte deja trazable qué vino del profesor y qué agregó
el equipo.

## D3 — Dapper, no Entity Framework
**Decisión:** todo el acceso a datos usa Dapper con SQL manual
parametrizado.
**Descartada:** Entity Framework Core con migraciones.
**Por qué:** lo prohíbe el Artículo 2; además Dapper deja ver exactamente
qué consulta se ejecuta, auditable contra `6_contracts.md`.

## D4 — Arquitectura en 4 capas, repetida idéntica en las 7 tablas
**Decisión:** Modelo → Petición → Repositorio → Servicio → Controller,
sin variación entre tablas.
**Descartada:** un Controller genérico que arme SQL dinámicamente según
el nombre de la tabla.
**Por qué:** rompería el Artículo 2 (SQL siempre parametrizado y visible,
no ensamblado dinámicamente) y perdería la validación específica de cada
tabla.

## D5 — Frontend en Razor Pages, no en una SPA aparte
**Decisión:** ASP.NET Core Razor Pages, mismo lenguaje que el backend.
**Descartada:** React, Angular o Vue como proyecto separado.
**Por qué:** el PDS del módulo permite React/Angular/Vue/HTML-JS; Razor
Pages cumple esa restricción sin introducir un segundo ecosistema bajo el
tiempo disponible. Queda abierta a revisión en una entrega futura.

## D6 — `aliado` usa `nit`, no `id`
**Decisión:** rutas y parámetros de `aliado` usan `nit`.
**Descartada:** normalizar a `id` en la API aunque la columna real sea
`nit`.
**Por qué:** el script define la columna como `nit`; renombrarla
generaría inconsistencia entre el contrato y la base de datos real.

## D7 — Repositorios de GitHub independientes
**Decisión:** `api-innovacion-curricular` y
`frontend-innovacion-curricular`, cada uno con ramas por integrante.
**Descartada:** un solo repositorio con ambos proyectos.
**Por qué:** lo exige el PDS explícitamente, y refuerza en la práctica el
Artículo 1 de separación de capas.
