# Constitución del proyecto

Módulo: innovación curricular — sistema de gestión de información del conocimiento universitario
Asignatura: diseño de software
Equipo: Carlos Adrián Rentería Machado, Cesar Luis Mosquera García

Este documento reúne las reglas que no cambian entre entregas. Las decisiones propias de cada entrega van en su propio `spec.md`, `plan.md` y `contracts.md`; lo que está aquí aplica siempre.

## 1. Principios no negociables

1. **Separación estricta backend/frontend.** El backend (API) es el único que se conecta a la base de datos. El frontend consume exclusivamente la API vía HTTP/JSON. Ninguna pantalla del frontend ejecuta SQL ni abre una conexión a PostgreSQL directamente.
2. **La API solo responde JSON.** No genera HTML. Cada endpoint usa el verbo HTTP correcto (GET, POST, PUT, DELETE) y responde con el código HTTP correspondiente (200, 201, 400, 404, 500).
3. **Borrado lógico, nunca físico.** Ninguna operación de la API ejecuta `DELETE FROM`. Eliminar un registro significa actualizar su columna `activo` a `false`. Los listados solo devuelven registros con `activo = true`, salvo que el endpoint diga explícitamente lo contrario.
4. **El ID se digita, no se genera solo.** El script de base de datos que entregó el profesor define las llaves primarias de las 22 tablas del módulo como `INT NOT NULL` sin autoincremento. El equipo decidió respetar el script tal como fue entregado en vez de cambiarlo a `SERIAL`. Esto implica que todo formulario de creación pide el ID, y la API valida que no exista antes de insertar.
5. **La columna `activo` no viene en el script original.** El script `innovacion_curricular.pg.sql` solo trae `activo` en `usuario` y `rol`. Antes de programar cualquier endpoint, el equipo agrega `activo BOOLEAN DEFAULT TRUE` a cada una de las 22 tablas del módulo mediante `ALTER TABLE`. Esto se hace una sola vez y se documenta en `db/`.
6. **Idioma del código: español.** Nombres de clases, métodos, variables, comentarios y mensajes de error van en español. Las palabras reservadas del lenguaje (`public`, `class`, `async`, etc.) se quedan en inglés porque son sintaxis de C#, no vocabulario del dominio.
7. **La especificación manda.** Si el código y el `spec.md` de una entrega no coinciden, se corrige el código. No se agregan endpoints, campos o pantallas que la entrega actual no pida, aunque parezcan buena idea — eso se propone para la siguiente entrega.

## 2. Stack técnico

- **Backend:** C# sobre ASP.NET Core.
- **Acceso a datos:** Dapper (SQL escrito a mano contra PostgreSQL), siguiendo el mismo patrón del proyecto de referencia del profesor.
- **Base de datos:** PostgreSQL, base `innovacion_curricular`, a partir del script entregado más el ajuste del punto 1.5.
- **Contenedores:** Docker y docker-compose, para que la base de datos y la API corran con un solo comando, sin instalar PostgreSQL ni el SDK de .NET localmente.
- **Documentación interactiva de la API:** Swagger, generado automáticamente por ASP.NET Core.
- **Frontend (a partir de la entrega correspondiente):** por definir; debe consumir la API por HTTP, nunca la base de datos.

## 3. Arquitectura por capas del backend

Cada tabla que la API expone sigue el mismo patrón de cuatro capas:

1. **Controllers** — capa HTTP. Recibe la petición, valida el formato básico, llama al servicio y traduce el resultado a un código HTTP. No contiene lógica de negocio ni SQL.
2. **Servicios** — capa de negocio. Aplica las reglas (por ejemplo, que el ID no esté repetido, que las llaves foráneas existan y estén activas). No sabe cómo se guarda el dato, solo qué reglas debe cumplir.
3. **Repositorios** — capa de datos. Ejecuta las consultas SQL contra PostgreSQL con Dapper. No conoce reglas de negocio, solo sabe leer y escribir.
4. **Modelos** — las clases que representan cada tabla (por ejemplo, `AreaConocimiento`), y las clases de petición (`Crear...`, `Actualizar...`) que reciben y validan lo que llega en el body.

Un error de validación (dato inválido, ID repetido) se traduce en un `400`. Un registro no encontrado se traduce en un `404`. Un error no previsto se traduce en un `500`.

## 4. Flujo de trabajo en Git

- Repositorio propio para la API (y, más adelante, otro independiente para el frontend), tal como exige el PDS.
- Cada integrante trabaja en su propia rama; nadie hace commits directos a `main`.
- Un integrante administra la rama `main` y hace merge cuando una parte del trabajo está lista y probada.
- Un commit describe qué se hizo, no "cambios" o "avance".

## 5. Qué significa que una entrega esté "terminada"

Una entrega se da por cerrada solo cuando:

1. Todos los endpoints de esa entrega responden lo que dice su `contracts.md`.
2. El borrado es lógico y verificable (el registro sigue en la base de datos con `activo = false`).
3. El proyecto corre completo con `docker compose up -d --build` sin pasos manuales adicionales.
4. Las ramas de los integrantes que aportaron a esa entrega están integradas en `main`.

## 6. Historial de cambios de este documento

| Versión | Fecha | Cambio |
|---|---|---|
| 1.0 | por definir | Versión inicial, alcance: entrega 1 (catálogos sin FK) |
