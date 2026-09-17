# 1_constitution.md

**Versión de la constitución:** 1.0 — vigente desde la Entrega 1.
Rige TODAS las entregas del módulo de innovación curricular; nada de aquí
cambia al pasar de una entrega a otra salvo por el artículo de enmiendas.

## Artículo 1 — Separación estricta backend / frontend
El frontend nunca se conecta a la base de datos. Toda lectura o escritura
pasa por la API vía HTTP/JSON. La API es la única capa con acceso a
PostgreSQL.

## Artículo 2 — La API solo habla JSON
Ningún endpoint devuelve HTML. Cada operación usa el verbo HTTP correcto
(`GET`, `POST`, `PUT`, `DELETE`) y responde con el código HTTP exacto que
define `6_contracts.md` de la entrega correspondiente.

## Artículo 3 — SQL siempre parametrizado, sin ORM completo
El acceso a datos se escribe con Dapper. Los valores viajan como
`@parametros`; jamás se concatenan directamente en el texto del SQL.
No se usa Entity Framework ni ningún ORM que genere el esquema.

## Artículo 4 — Borrado lógico, nunca físico
Ninguna operación ejecuta `DELETE FROM`. Eliminar un registro significa
actualizar su columna `activo` a `false`. Todo `SELECT` de listado filtra
por `activo = true`, salvo que el contrato de la entrega diga lo
contrario explícitamente.

## Artículo 5 — El script de base de datos es un artefacto dado
El script SQL que entrega el profesor (`db/00_innovacion_curricular.pg.sql`)
no se modifica. Si una entrega necesita una columna que el script no
trae (como `activo`), se agrega en un script **adicional y numerado**
(`db/01_alter_activo.sql`, `db/02_...`), nunca editando el original.

## Artículo 6 — El id no se genera solo, salvo que el script lo diga
Las llaves primarias de las 22 tablas propias del módulo están definidas
en el script como `INT` sin autoincremento. La API las recibe en la
petición de creación y valida que no estén repetidas antes de insertar.
(Las 3 tablas de usuarios/roles sí son `SERIAL`, porque así las definió
el script.)

## Artículo 7 — Arquitectura en capas, siempre las mismas cuatro
Toda entidad de la API sigue el mismo patrón: **Controllers** (HTTP) →
**Servicios** (reglas de negocio) → **Repositorios** (SQL con Dapper) →
**Modelos** (la clase que representa la tabla). Ninguna capa se salta:
un Controller nunca ejecuta SQL directo, un Repositorio nunca valida
reglas de negocio.

## Artículo 8 — Idioma del código: español
Nombres de clases, métodos, variables, comentarios y mensajes de error
van en español. Las palabras reservadas del lenguaje (`public`, `class`,
`async`…) se quedan en inglés porque son sintaxis, no vocabulario del
dominio.

## Artículo 9 — La especificación manda
Si el código hace algo que la spec de la entrega no pide, sobra y se
retira. Si la spec pide algo que el código no hace, falta y se agrega.
No se anticipan funcionalidades de entregas futuras (YAGNI): la Entrega 1
no construye nada de la Entrega 2 "por si acaso".

## Artículo 10 — Un solo comando levanta todo
El proyecto completo (base de datos, API, frontend) arranca con
`docker compose up -d --build`, parado en la raíz del repositorio. Nadie
necesita instalar PostgreSQL ni el SDK de .NET localmente para probarlo.

## Artículo 11 — Control de versiones por rama
Repositorio independiente para la API y otro para el frontend. Nadie
hace commits directos a `main`. Cada integrante trabaja en su propia
rama y la integra a `main` mediante Pull Request cuando su parte está
lista y probada.

## Artículo 12 — Cerrado es cerrado
Una entrega con sus criterios de aceptación en verde no se reabre para
agregarle cosas nuevas; los ajustes van a la entrega siguiente. Si algo
quedó mal especificado, se anota como deuda de spec en el
`4_research.md` de la entrega que lo corrige.

## Artículo de enmiendas
Cualquier cambio a un artículo de esta constitución se propone primero en
el `4_research.md` de la entrega que lo necesita, con su justificación.
Si se aprueba, esta constitución sube de versión (1.1, 1.2…) y el cambio
queda registrado en la tabla de abajo.

## Historial de cambios

| Versión | Fecha | Cambio |
|---|---|---|
| 1.0 | (reconstruida junto con el spec kit de la Entrega 1) | Versión inicial, alcance: todas las entregas del módulo |
