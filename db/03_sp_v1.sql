-- Docker ejecuta este archivo DESPUÉS de 02_alter_v2.sql (orden alfabético).
-- Las 7 tablas de la v1 pasan a funciones sp_*: 5 verbos por tabla, 35 en total
-- (constitución Art. 2). Desde aquí el repositorio de C# solo llama
-- SELECT * FROM sp_xxx(...); ninguna sentencia sobre tablas vive en C#.
--
-- Por qué FUNCIONES y no PROCEDURE: un PROCEDURE no devuelve filas, y crear y
-- actualizar deben devolver la fila tal como quedó guardada (D-v2-1).
--
-- Reglas comunes a las 7 tablas:
--  * Los tipos de los parámetros salen de la propia columna (tabla.columna%TYPE),
--    así un ensanche como el de disciplina (02) no deja las funciones desfasadas.
--  * Listar y consultar solo ven filas activas (RF2).
--  * Crear NO comprueba duplicados a mano: la llave primaria lo hace y lanza el
--    error 23505, que el middleware de la API traduce a 409. Una comprobación
--    previa tendría una carrera entre «no existe» y el INSERT (D-v2-3).
--    Un id retirado también cuenta como repetido, igual que en la v1.
--  * Actualizar devuelve CERO filas si el id no existe o está retirado; la API
--    lo traduce a 404.
--  * Eliminar es un UPDATE activo = FALSE (borrado lógico) y devuelve TRUE si
--    retiró algo, FALSE si no había nada activo con ese id.
--  * Todas son idempotentes (CREATE OR REPLACE): se pueden volver a aplicar sin
--    recrear la base.

-- ============================================================
-- area_conocimiento (áreas de conocimiento)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_area_conocimiento()
RETURNS SETOF area_conocimiento
LANGUAGE sql STABLE
AS $$
    SELECT * FROM area_conocimiento WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_area_conocimiento(p_id area_conocimiento.id%TYPE)
RETURNS SETOF area_conocimiento
LANGUAGE sql STABLE
AS $$
    SELECT * FROM area_conocimiento WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_area_conocimiento(
    p_id area_conocimiento.id%TYPE,
    p_gran_area area_conocimiento.gran_area%TYPE,
    p_area area_conocimiento.area%TYPE,
    p_disciplina area_conocimiento.disciplina%TYPE
)
RETURNS area_conocimiento
LANGUAGE sql
AS $$
    INSERT INTO area_conocimiento (id, gran_area, area, disciplina, activo)
    VALUES (p_id, p_gran_area, p_area, p_disciplina, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_area_conocimiento(
    p_id area_conocimiento.id%TYPE,
    p_gran_area area_conocimiento.gran_area%TYPE,
    p_area area_conocimiento.area%TYPE,
    p_disciplina area_conocimiento.disciplina%TYPE
)
RETURNS SETOF area_conocimiento
LANGUAGE sql
AS $$
    UPDATE area_conocimiento SET
        gran_area = p_gran_area,
        area = p_area,
        disciplina = p_disciplina
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_area_conocimiento(p_id area_conocimiento.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE area_conocimiento SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- universidad (universidades)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_universidad()
RETURNS SETOF universidad
LANGUAGE sql STABLE
AS $$
    SELECT * FROM universidad WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_universidad(p_id universidad.id%TYPE)
RETURNS SETOF universidad
LANGUAGE sql STABLE
AS $$
    SELECT * FROM universidad WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_universidad(
    p_id universidad.id%TYPE,
    p_nombre universidad.nombre%TYPE,
    p_tipo universidad.tipo%TYPE,
    p_ciudad universidad.ciudad%TYPE
)
RETURNS universidad
LANGUAGE sql
AS $$
    INSERT INTO universidad (id, nombre, tipo, ciudad, activo)
    VALUES (p_id, p_nombre, p_tipo, p_ciudad, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_universidad(
    p_id universidad.id%TYPE,
    p_nombre universidad.nombre%TYPE,
    p_tipo universidad.tipo%TYPE,
    p_ciudad universidad.ciudad%TYPE
)
RETURNS SETOF universidad
LANGUAGE sql
AS $$
    UPDATE universidad SET
        nombre = p_nombre,
        tipo = p_tipo,
        ciudad = p_ciudad
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_universidad(p_id universidad.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE universidad SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- aspecto_normativo (aspectos normativos)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_aspecto_normativo()
RETURNS SETOF aspecto_normativo
LANGUAGE sql STABLE
AS $$
    SELECT * FROM aspecto_normativo WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_aspecto_normativo(p_id aspecto_normativo.id%TYPE)
RETURNS SETOF aspecto_normativo
LANGUAGE sql STABLE
AS $$
    SELECT * FROM aspecto_normativo WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_aspecto_normativo(
    p_id aspecto_normativo.id%TYPE,
    p_tipo aspecto_normativo.tipo%TYPE,
    p_descripcion aspecto_normativo.descripcion%TYPE,
    p_fuente aspecto_normativo.fuente%TYPE
)
RETURNS aspecto_normativo
LANGUAGE sql
AS $$
    INSERT INTO aspecto_normativo (id, tipo, descripcion, fuente, activo)
    VALUES (p_id, p_tipo, p_descripcion, p_fuente, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_aspecto_normativo(
    p_id aspecto_normativo.id%TYPE,
    p_tipo aspecto_normativo.tipo%TYPE,
    p_descripcion aspecto_normativo.descripcion%TYPE,
    p_fuente aspecto_normativo.fuente%TYPE
)
RETURNS SETOF aspecto_normativo
LANGUAGE sql
AS $$
    UPDATE aspecto_normativo SET
        tipo = p_tipo,
        descripcion = p_descripcion,
        fuente = p_fuente
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_aspecto_normativo(p_id aspecto_normativo.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE aspecto_normativo SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;
-- ============================================================
-- practica_estrategia (prácticas y estrategias)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_practica_estrategia()
RETURNS SETOF practica_estrategia
LANGUAGE sql STABLE
AS $$
    SELECT * FROM practica_estrategia WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_practica_estrategia(p_id practica_estrategia.id%TYPE)
RETURNS SETOF practica_estrategia
LANGUAGE sql STABLE
AS $$
    SELECT * FROM practica_estrategia WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_practica_estrategia(
    p_id practica_estrategia.id%TYPE,
    p_tipo practica_estrategia.tipo%TYPE,
    p_nombre practica_estrategia.nombre%TYPE,
    p_descripcion practica_estrategia.descripcion%TYPE
)
RETURNS practica_estrategia
LANGUAGE sql
AS $$
    INSERT INTO practica_estrategia (id, tipo, nombre, descripcion, activo)
    VALUES (p_id, p_tipo, p_nombre, p_descripcion, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_practica_estrategia(
    p_id practica_estrategia.id%TYPE,
    p_tipo practica_estrategia.tipo%TYPE,
    p_nombre practica_estrategia.nombre%TYPE,
    p_descripcion practica_estrategia.descripcion%TYPE
)
RETURNS SETOF practica_estrategia
LANGUAGE sql
AS $$
    UPDATE practica_estrategia SET
        tipo = p_tipo,
        nombre = p_nombre,
        descripcion = p_descripcion
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_practica_estrategia(p_id practica_estrategia.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE practica_estrategia SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- enfoque (enfoques)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_enfoque()
RETURNS SETOF enfoque
LANGUAGE sql STABLE
AS $$
    SELECT * FROM enfoque WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_enfoque(p_id enfoque.id%TYPE)
RETURNS SETOF enfoque
LANGUAGE sql STABLE
AS $$
    SELECT * FROM enfoque WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_enfoque(
    p_id enfoque.id%TYPE,
    p_nombre enfoque.nombre%TYPE,
    p_descripcion enfoque.descripcion%TYPE
)
RETURNS enfoque
LANGUAGE sql
AS $$
    INSERT INTO enfoque (id, nombre, descripcion, activo)
    VALUES (p_id, p_nombre, p_descripcion, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_enfoque(
    p_id enfoque.id%TYPE,
    p_nombre enfoque.nombre%TYPE,
    p_descripcion enfoque.descripcion%TYPE
)
RETURNS SETOF enfoque
LANGUAGE sql
AS $$
    UPDATE enfoque SET
        nombre = p_nombre,
        descripcion = p_descripcion
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_enfoque(p_id enfoque.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE enfoque SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- car_innovacion (características de innovación)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_car_innovacion()
RETURNS SETOF car_innovacion
LANGUAGE sql STABLE
AS $$
    SELECT * FROM car_innovacion WHERE activo ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_car_innovacion(p_id car_innovacion.id%TYPE)
RETURNS SETOF car_innovacion
LANGUAGE sql STABLE
AS $$
    SELECT * FROM car_innovacion WHERE id = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_car_innovacion(
    p_id car_innovacion.id%TYPE,
    p_nombre car_innovacion.nombre%TYPE,
    p_descripcion car_innovacion.descripcion%TYPE,
    p_tipo car_innovacion.tipo%TYPE
)
RETURNS car_innovacion
LANGUAGE sql
AS $$
    INSERT INTO car_innovacion (id, nombre, descripcion, tipo, activo)
    VALUES (p_id, p_nombre, p_descripcion, p_tipo, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_car_innovacion(
    p_id car_innovacion.id%TYPE,
    p_nombre car_innovacion.nombre%TYPE,
    p_descripcion car_innovacion.descripcion%TYPE,
    p_tipo car_innovacion.tipo%TYPE
)
RETURNS SETOF car_innovacion
LANGUAGE sql
AS $$
    UPDATE car_innovacion SET
        nombre = p_nombre,
        descripcion = p_descripcion,
        tipo = p_tipo
    WHERE id = p_id AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_car_innovacion(p_id car_innovacion.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE car_innovacion SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- aliado (aliados)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_aliado()
RETURNS SETOF aliado
LANGUAGE sql STABLE
AS $$
    SELECT * FROM aliado WHERE activo ORDER BY nit;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_aliado(p_nit aliado.nit%TYPE)
RETURNS SETOF aliado
LANGUAGE sql STABLE
AS $$
    SELECT * FROM aliado WHERE nit = p_nit AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_aliado(
    p_nit aliado.nit%TYPE,
    p_razon_social aliado.razon_social%TYPE,
    p_nombre_contacto aliado.nombre_contacto%TYPE,
    p_correo aliado.correo%TYPE,
    p_telefono aliado.telefono%TYPE,
    p_ciudad aliado.ciudad%TYPE
)
RETURNS aliado
LANGUAGE sql
AS $$
    INSERT INTO aliado (nit, razon_social, nombre_contacto, correo, telefono, ciudad, activo)
    VALUES (p_nit, p_razon_social, p_nombre_contacto, p_correo, p_telefono, p_ciudad, TRUE)
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_aliado(
    p_nit aliado.nit%TYPE,
    p_razon_social aliado.razon_social%TYPE,
    p_nombre_contacto aliado.nombre_contacto%TYPE,
    p_correo aliado.correo%TYPE,
    p_telefono aliado.telefono%TYPE,
    p_ciudad aliado.ciudad%TYPE
)
RETURNS SETOF aliado
LANGUAGE sql
AS $$
    UPDATE aliado SET
        razon_social = p_razon_social,
        nombre_contacto = p_nombre_contacto,
        correo = p_correo,
        telefono = p_telefono,
        ciudad = p_ciudad
    WHERE nit = p_nit AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_aliado(p_nit aliado.nit%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE aliado SET activo = FALSE WHERE nit = p_nit AND activo;
    RETURN FOUND;
END;
$$;