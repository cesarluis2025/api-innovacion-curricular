-- Docker ejecuta este archivo DESPUÉS de 03_sp_v1.sql (orden alfabético).
-- Funciones sp_* de las 7 entidades con llave foránea: facultad, programa,
-- acreditacion, registro_calificado, activ_academica, pasantia y premio
-- (5 verbos cada una, 35 funciones).
--
-- Lo que añaden respecto a 03_sp_v1.sql:
--  * Listar y consultar traen además el NOMBRE del padre (JOIN), para que la
--    pantalla muestre «Facultad de Ingeniería» y no un número (VERSION_2.md §6).
--    Crear y actualizar devuelven la fila tal cual quedó guardada, sin ese nombre.
--  * Crear y actualizar comprueban que el padre exista Y esté activo. La llave
--    foránea del script solo comprueba que exista: un padre retirado seguiría
--    aceptando hijos. Se lanza el MISMO código 23503 que una llave foránea real,
--    con la tabla hija y el campo, para que el middleware responda 409 por un
--    único camino (D-v2-13).
--  * Una FK opcional (activ_academica.disenio) solo se comprueba si viene
--    informada: NULL significa «(ninguna)».
--  * facultad.total_programas NO es parámetro de crear ni actualizar: solo lo
--    escribe el disparador (05_disparadores.sql).
--  * activ_academica.espejo es SMALLINT, pero el parámetro es INT: PostgreSQL no
--    acepta un entero normal donde la función espera smallint, y C# manda int.
--    La conversión ocurre dentro, en el INSERT/UPDATE.
--  * Idempotentes (CREATE OR REPLACE).

-- ============================================================
-- facultad (facultades)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_facultad()
RETURNS TABLE (
    id facultad.id%TYPE,
    nombre facultad.nombre%TYPE,
    tipo facultad.tipo%TYPE,
    fecha_fun facultad.fecha_fun%TYPE,
    universidad facultad.universidad%TYPE,
    activo BOOLEAN,
    total_programas facultad.total_programas%TYPE,
    universidad_nombre universidad.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.tipo,
        h.fecha_fun,
        h.universidad,
        h.activo,
        h.total_programas,
        p.nombre
    FROM facultad h
    JOIN universidad p ON p.id = h.universidad
    WHERE h.activo
    ORDER BY h.id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_facultad(p_id facultad.id%TYPE)
RETURNS TABLE (
    id facultad.id%TYPE,
    nombre facultad.nombre%TYPE,
    tipo facultad.tipo%TYPE,
    fecha_fun facultad.fecha_fun%TYPE,
    universidad facultad.universidad%TYPE,
    activo BOOLEAN,
    total_programas facultad.total_programas%TYPE,
    universidad_nombre universidad.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.tipo,
        h.fecha_fun,
        h.universidad,
        h.activo,
        h.total_programas,
        p.nombre
    FROM facultad h
    JOIN universidad p ON p.id = h.universidad
    WHERE h.id = p_id AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_facultad(
    p_id facultad.id%TYPE,
    p_nombre facultad.nombre%TYPE,
    p_tipo facultad.tipo%TYPE,
    p_fecha_fun facultad.fecha_fun%TYPE,
    p_universidad facultad.universidad%TYPE
)
RETURNS facultad
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila facultad;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM universidad WHERE id = p_universidad AND activo) THEN
        RAISE EXCEPTION 'el campo universidad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'facultad', COLUMN = 'universidad';
    END IF;
    INSERT INTO facultad (id, nombre, tipo, fecha_fun, universidad, activo)
    VALUES (p_id, p_nombre, p_tipo, p_fecha_fun, p_universidad, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_facultad(
    p_id facultad.id%TYPE,
    p_nombre facultad.nombre%TYPE,
    p_tipo facultad.tipo%TYPE,
    p_fecha_fun facultad.fecha_fun%TYPE,
    p_universidad facultad.universidad%TYPE
)
RETURNS SETOF facultad
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila facultad;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM universidad WHERE id = p_universidad AND activo) THEN
        RAISE EXCEPTION 'el campo universidad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'facultad', COLUMN = 'universidad';
    END IF;
    UPDATE facultad SET
        nombre = p_nombre,
        tipo = p_tipo,
        fecha_fun = p_fecha_fun,
        universidad = p_universidad
    WHERE id = p_id AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_facultad(p_id facultad.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE facultad SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- programa (programas)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_programa()
RETURNS TABLE (
    id programa.id%TYPE,
    nombre programa.nombre%TYPE,
    tipo programa.tipo%TYPE,
    nivel programa.nivel%TYPE,
    fecha_creacion programa.fecha_creacion%TYPE,
    fecha_cierre programa.fecha_cierre%TYPE,
    numero_cohortes programa.numero_cohortes%TYPE,
    cant_graduados programa.cant_graduados%TYPE,
    fecha_actualizacion programa.fecha_actualizacion%TYPE,
    ciudad programa.ciudad%TYPE,
    facultad programa.facultad%TYPE,
    activo BOOLEAN,
    facultad_nombre facultad.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.tipo,
        h.nivel,
        h.fecha_creacion,
        h.fecha_cierre,
        h.numero_cohortes,
        h.cant_graduados,
        h.fecha_actualizacion,
        h.ciudad,
        h.facultad,
        h.activo,
        p.nombre
    FROM programa h
    JOIN facultad p ON p.id = h.facultad
    WHERE h.activo
    ORDER BY h.id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_programa(p_id programa.id%TYPE)
RETURNS TABLE (
    id programa.id%TYPE,
    nombre programa.nombre%TYPE,
    tipo programa.tipo%TYPE,
    nivel programa.nivel%TYPE,
    fecha_creacion programa.fecha_creacion%TYPE,
    fecha_cierre programa.fecha_cierre%TYPE,
    numero_cohortes programa.numero_cohortes%TYPE,
    cant_graduados programa.cant_graduados%TYPE,
    fecha_actualizacion programa.fecha_actualizacion%TYPE,
    ciudad programa.ciudad%TYPE,
    facultad programa.facultad%TYPE,
    activo BOOLEAN,
    facultad_nombre facultad.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.tipo,
        h.nivel,
        h.fecha_creacion,
        h.fecha_cierre,
        h.numero_cohortes,
        h.cant_graduados,
        h.fecha_actualizacion,
        h.ciudad,
        h.facultad,
        h.activo,
        p.nombre
    FROM programa h
    JOIN facultad p ON p.id = h.facultad
    WHERE h.id = p_id AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_programa(
    p_id programa.id%TYPE,
    p_nombre programa.nombre%TYPE,
    p_tipo programa.tipo%TYPE,
    p_nivel programa.nivel%TYPE,
    p_fecha_creacion programa.fecha_creacion%TYPE,
    p_fecha_cierre programa.fecha_cierre%TYPE,
    p_numero_cohortes programa.numero_cohortes%TYPE,
    p_cant_graduados programa.cant_graduados%TYPE,
    p_fecha_actualizacion programa.fecha_actualizacion%TYPE,
    p_ciudad programa.ciudad%TYPE,
    p_facultad programa.facultad%TYPE
)
RETURNS programa
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila programa;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM facultad WHERE id = p_facultad AND activo) THEN
        RAISE EXCEPTION 'el campo facultad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa', COLUMN = 'facultad';
    END IF;
    INSERT INTO programa (id, nombre, tipo, nivel, fecha_creacion, fecha_cierre, numero_cohortes, cant_graduados, fecha_actualizacion, ciudad, facultad, activo)
    VALUES (p_id, p_nombre, p_tipo, p_nivel, p_fecha_creacion, p_fecha_cierre, p_numero_cohortes, p_cant_graduados, p_fecha_actualizacion, p_ciudad, p_facultad, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_programa(
    p_id programa.id%TYPE,
    p_nombre programa.nombre%TYPE,
    p_tipo programa.tipo%TYPE,
    p_nivel programa.nivel%TYPE,
    p_fecha_creacion programa.fecha_creacion%TYPE,
    p_fecha_cierre programa.fecha_cierre%TYPE,
    p_numero_cohortes programa.numero_cohortes%TYPE,
    p_cant_graduados programa.cant_graduados%TYPE,
    p_fecha_actualizacion programa.fecha_actualizacion%TYPE,
    p_ciudad programa.ciudad%TYPE,
    p_facultad programa.facultad%TYPE
)
RETURNS SETOF programa
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila programa;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM facultad WHERE id = p_facultad AND activo) THEN
        RAISE EXCEPTION 'el campo facultad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa', COLUMN = 'facultad';
    END IF;
    UPDATE programa SET
        nombre = p_nombre,
        tipo = p_tipo,
        nivel = p_nivel,
        fecha_creacion = p_fecha_creacion,
        fecha_cierre = p_fecha_cierre,
        numero_cohortes = p_numero_cohortes,
        cant_graduados = p_cant_graduados,
        fecha_actualizacion = p_fecha_actualizacion,
        ciudad = p_ciudad,
        facultad = p_facultad
    WHERE id = p_id AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_programa(p_id programa.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE programa SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- acreditacion (acreditaciones)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_acreditacion()
RETURNS TABLE (
    resolucion acreditacion.resolucion%TYPE,
    tipo acreditacion.tipo%TYPE,
    calificacion acreditacion.calificacion%TYPE,
    fecha_inicio acreditacion.fecha_inicio%TYPE,
    fecha_fin acreditacion.fecha_fin%TYPE,
    programa acreditacion.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.resolucion,
        h.tipo,
        h.calificacion,
        h.fecha_inicio,
        h.fecha_fin,
        h.programa,
        h.activo,
        p.nombre
    FROM acreditacion h
    JOIN programa p ON p.id = h.programa
    WHERE h.activo
    ORDER BY h.resolucion;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_acreditacion(p_resolucion acreditacion.resolucion%TYPE)
RETURNS TABLE (
    resolucion acreditacion.resolucion%TYPE,
    tipo acreditacion.tipo%TYPE,
    calificacion acreditacion.calificacion%TYPE,
    fecha_inicio acreditacion.fecha_inicio%TYPE,
    fecha_fin acreditacion.fecha_fin%TYPE,
    programa acreditacion.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.resolucion,
        h.tipo,
        h.calificacion,
        h.fecha_inicio,
        h.fecha_fin,
        h.programa,
        h.activo,
        p.nombre
    FROM acreditacion h
    JOIN programa p ON p.id = h.programa
    WHERE h.resolucion = p_resolucion AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_acreditacion(
    p_resolucion acreditacion.resolucion%TYPE,
    p_tipo acreditacion.tipo%TYPE,
    p_calificacion acreditacion.calificacion%TYPE,
    p_fecha_inicio acreditacion.fecha_inicio%TYPE,
    p_fecha_fin acreditacion.fecha_fin%TYPE,
    p_programa acreditacion.programa%TYPE
)
RETURNS acreditacion
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila acreditacion;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'acreditacion', COLUMN = 'programa';
    END IF;
    INSERT INTO acreditacion (resolucion, tipo, calificacion, fecha_inicio, fecha_fin, programa, activo)
    VALUES (p_resolucion, p_tipo, p_calificacion, p_fecha_inicio, p_fecha_fin, p_programa, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_acreditacion(
    p_resolucion acreditacion.resolucion%TYPE,
    p_tipo acreditacion.tipo%TYPE,
    p_calificacion acreditacion.calificacion%TYPE,
    p_fecha_inicio acreditacion.fecha_inicio%TYPE,
    p_fecha_fin acreditacion.fecha_fin%TYPE,
    p_programa acreditacion.programa%TYPE
)
RETURNS SETOF acreditacion
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila acreditacion;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'acreditacion', COLUMN = 'programa';
    END IF;
    UPDATE acreditacion SET
        tipo = p_tipo,
        calificacion = p_calificacion,
        fecha_inicio = p_fecha_inicio,
        fecha_fin = p_fecha_fin,
        programa = p_programa
    WHERE resolucion = p_resolucion AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_acreditacion(p_resolucion acreditacion.resolucion%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE acreditacion SET activo = FALSE WHERE resolucion = p_resolucion AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- registro_calificado (registros calificados)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_registro_calificado()
RETURNS TABLE (
    codigo registro_calificado.codigo%TYPE,
    cant_creditos registro_calificado.cant_creditos%TYPE,
    hora_acom registro_calificado.hora_acom%TYPE,
    hora_ind registro_calificado.hora_ind%TYPE,
    metodologia registro_calificado.metodologia%TYPE,
    fecha_inicio registro_calificado.fecha_inicio%TYPE,
    fecha_fin registro_calificado.fecha_fin%TYPE,
    duracion_anios registro_calificado.duracion_anios%TYPE,
    duracion_semestres registro_calificado.duracion_semestres%TYPE,
    tipo_titulacion registro_calificado.tipo_titulacion%TYPE,
    programa registro_calificado.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.codigo,
        h.cant_creditos,
        h.hora_acom,
        h.hora_ind,
        h.metodologia,
        h.fecha_inicio,
        h.fecha_fin,
        h.duracion_anios,
        h.duracion_semestres,
        h.tipo_titulacion,
        h.programa,
        h.activo,
        p.nombre
    FROM registro_calificado h
    JOIN programa p ON p.id = h.programa
    WHERE h.activo
    ORDER BY h.codigo;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_registro_calificado(p_codigo registro_calificado.codigo%TYPE)
RETURNS TABLE (
    codigo registro_calificado.codigo%TYPE,
    cant_creditos registro_calificado.cant_creditos%TYPE,
    hora_acom registro_calificado.hora_acom%TYPE,
    hora_ind registro_calificado.hora_ind%TYPE,
    metodologia registro_calificado.metodologia%TYPE,
    fecha_inicio registro_calificado.fecha_inicio%TYPE,
    fecha_fin registro_calificado.fecha_fin%TYPE,
    duracion_anios registro_calificado.duracion_anios%TYPE,
    duracion_semestres registro_calificado.duracion_semestres%TYPE,
    tipo_titulacion registro_calificado.tipo_titulacion%TYPE,
    programa registro_calificado.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.codigo,
        h.cant_creditos,
        h.hora_acom,
        h.hora_ind,
        h.metodologia,
        h.fecha_inicio,
        h.fecha_fin,
        h.duracion_anios,
        h.duracion_semestres,
        h.tipo_titulacion,
        h.programa,
        h.activo,
        p.nombre
    FROM registro_calificado h
    JOIN programa p ON p.id = h.programa
    WHERE h.codigo = p_codigo AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_registro_calificado(
    p_codigo registro_calificado.codigo%TYPE,
    p_cant_creditos registro_calificado.cant_creditos%TYPE,
    p_hora_acom registro_calificado.hora_acom%TYPE,
    p_hora_ind registro_calificado.hora_ind%TYPE,
    p_metodologia registro_calificado.metodologia%TYPE,
    p_fecha_inicio registro_calificado.fecha_inicio%TYPE,
    p_fecha_fin registro_calificado.fecha_fin%TYPE,
    p_duracion_anios registro_calificado.duracion_anios%TYPE,
    p_duracion_semestres registro_calificado.duracion_semestres%TYPE,
    p_tipo_titulacion registro_calificado.tipo_titulacion%TYPE,
    p_programa registro_calificado.programa%TYPE
)
RETURNS registro_calificado
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila registro_calificado;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'registro_calificado', COLUMN = 'programa';
    END IF;
    INSERT INTO registro_calificado (codigo, cant_creditos, hora_acom, hora_ind, metodologia, fecha_inicio, fecha_fin, duracion_anios, duracion_semestres, tipo_titulacion, programa, activo)
    VALUES (p_codigo, p_cant_creditos, p_hora_acom, p_hora_ind, p_metodologia, p_fecha_inicio, p_fecha_fin, p_duracion_anios, p_duracion_semestres, p_tipo_titulacion, p_programa, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_registro_calificado(
    p_codigo registro_calificado.codigo%TYPE,
    p_cant_creditos registro_calificado.cant_creditos%TYPE,
    p_hora_acom registro_calificado.hora_acom%TYPE,
    p_hora_ind registro_calificado.hora_ind%TYPE,
    p_metodologia registro_calificado.metodologia%TYPE,
    p_fecha_inicio registro_calificado.fecha_inicio%TYPE,
    p_fecha_fin registro_calificado.fecha_fin%TYPE,
    p_duracion_anios registro_calificado.duracion_anios%TYPE,
    p_duracion_semestres registro_calificado.duracion_semestres%TYPE,
    p_tipo_titulacion registro_calificado.tipo_titulacion%TYPE,
    p_programa registro_calificado.programa%TYPE
)
RETURNS SETOF registro_calificado
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila registro_calificado;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'registro_calificado', COLUMN = 'programa';
    END IF;
    UPDATE registro_calificado SET
        cant_creditos = p_cant_creditos,
        hora_acom = p_hora_acom,
        hora_ind = p_hora_ind,
        metodologia = p_metodologia,
        fecha_inicio = p_fecha_inicio,
        fecha_fin = p_fecha_fin,
        duracion_anios = p_duracion_anios,
        duracion_semestres = p_duracion_semestres,
        tipo_titulacion = p_tipo_titulacion,
        programa = p_programa
    WHERE codigo = p_codigo AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_registro_calificado(p_codigo registro_calificado.codigo%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE registro_calificado SET activo = FALSE WHERE codigo = p_codigo AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- activ_academica (actividades académicas)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_activ_academica()
RETURNS TABLE (
    id activ_academica.id%TYPE,
    nombre activ_academica.nombre%TYPE,
    num_creditos activ_academica.num_creditos%TYPE,
    tipo activ_academica.tipo%TYPE,
    area_formacion activ_academica.area_formacion%TYPE,
    h_acom activ_academica.h_acom%TYPE,
    h_indep activ_academica.h_indep%TYPE,
    idioma activ_academica.idioma%TYPE,
    espejo activ_academica.espejo%TYPE,
    entidad_espejo activ_academica.entidad_espejo%TYPE,
    pais_espejo activ_academica.pais_espejo%TYPE,
    disenio activ_academica.disenio%TYPE,
    activo BOOLEAN,
    disenio_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.num_creditos,
        h.tipo,
        h.area_formacion,
        h.h_acom,
        h.h_indep,
        h.idioma,
        h.espejo,
        h.entidad_espejo,
        h.pais_espejo,
        h.disenio,
        h.activo,
        p.nombre
    FROM activ_academica h
    LEFT JOIN programa p ON p.id = h.disenio
    WHERE h.activo
    ORDER BY h.id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_activ_academica(p_id activ_academica.id%TYPE)
RETURNS TABLE (
    id activ_academica.id%TYPE,
    nombre activ_academica.nombre%TYPE,
    num_creditos activ_academica.num_creditos%TYPE,
    tipo activ_academica.tipo%TYPE,
    area_formacion activ_academica.area_formacion%TYPE,
    h_acom activ_academica.h_acom%TYPE,
    h_indep activ_academica.h_indep%TYPE,
    idioma activ_academica.idioma%TYPE,
    espejo activ_academica.espejo%TYPE,
    entidad_espejo activ_academica.entidad_espejo%TYPE,
    pais_espejo activ_academica.pais_espejo%TYPE,
    disenio activ_academica.disenio%TYPE,
    activo BOOLEAN,
    disenio_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.num_creditos,
        h.tipo,
        h.area_formacion,
        h.h_acom,
        h.h_indep,
        h.idioma,
        h.espejo,
        h.entidad_espejo,
        h.pais_espejo,
        h.disenio,
        h.activo,
        p.nombre
    FROM activ_academica h
    LEFT JOIN programa p ON p.id = h.disenio
    WHERE h.id = p_id AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_activ_academica(
    p_id activ_academica.id%TYPE,
    p_nombre activ_academica.nombre%TYPE,
    p_num_creditos activ_academica.num_creditos%TYPE,
    p_tipo activ_academica.tipo%TYPE,
    p_area_formacion activ_academica.area_formacion%TYPE,
    p_h_acom activ_academica.h_acom%TYPE,
    p_h_indep activ_academica.h_indep%TYPE,
    p_idioma activ_academica.idioma%TYPE,
    p_espejo INT,
    p_entidad_espejo activ_academica.entidad_espejo%TYPE,
    p_pais_espejo activ_academica.pais_espejo%TYPE,
    p_disenio activ_academica.disenio%TYPE
)
RETURNS activ_academica
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila activ_academica;
BEGIN
    IF p_disenio IS NOT NULL AND NOT EXISTS (SELECT 1 FROM programa WHERE id = p_disenio AND activo) THEN
        RAISE EXCEPTION 'el campo disenio no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'activ_academica', COLUMN = 'disenio';
    END IF;
    INSERT INTO activ_academica (id, nombre, num_creditos, tipo, area_formacion, h_acom, h_indep, idioma, espejo, entidad_espejo, pais_espejo, disenio, activo)
    VALUES (p_id, p_nombre, p_num_creditos, p_tipo, p_area_formacion, p_h_acom, p_h_indep, p_idioma, p_espejo, p_entidad_espejo, p_pais_espejo, p_disenio, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_activ_academica(
    p_id activ_academica.id%TYPE,
    p_nombre activ_academica.nombre%TYPE,
    p_num_creditos activ_academica.num_creditos%TYPE,
    p_tipo activ_academica.tipo%TYPE,
    p_area_formacion activ_academica.area_formacion%TYPE,
    p_h_acom activ_academica.h_acom%TYPE,
    p_h_indep activ_academica.h_indep%TYPE,
    p_idioma activ_academica.idioma%TYPE,
    p_espejo INT,
    p_entidad_espejo activ_academica.entidad_espejo%TYPE,
    p_pais_espejo activ_academica.pais_espejo%TYPE,
    p_disenio activ_academica.disenio%TYPE
)
RETURNS SETOF activ_academica
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila activ_academica;
BEGIN
    IF p_disenio IS NOT NULL AND NOT EXISTS (SELECT 1 FROM programa WHERE id = p_disenio AND activo) THEN
        RAISE EXCEPTION 'el campo disenio no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'activ_academica', COLUMN = 'disenio';
    END IF;
    UPDATE activ_academica SET
        nombre = p_nombre,
        num_creditos = p_num_creditos,
        tipo = p_tipo,
        area_formacion = p_area_formacion,
        h_acom = p_h_acom,
        h_indep = p_h_indep,
        idioma = p_idioma,
        espejo = p_espejo,
        entidad_espejo = p_entidad_espejo,
        pais_espejo = p_pais_espejo,
        disenio = p_disenio
    WHERE id = p_id AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_activ_academica(p_id activ_academica.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE activ_academica SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- pasantia (pasantías)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_pasantia()
RETURNS TABLE (
    id pasantia.id%TYPE,
    nombre pasantia.nombre%TYPE,
    pais pasantia.pais%TYPE,
    empresa pasantia.empresa%TYPE,
    descripcion pasantia.descripcion%TYPE,
    programa pasantia.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.pais,
        h.empresa,
        h.descripcion,
        h.programa,
        h.activo,
        p.nombre
    FROM pasantia h
    JOIN programa p ON p.id = h.programa
    WHERE h.activo
    ORDER BY h.id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_pasantia(p_id pasantia.id%TYPE)
RETURNS TABLE (
    id pasantia.id%TYPE,
    nombre pasantia.nombre%TYPE,
    pais pasantia.pais%TYPE,
    empresa pasantia.empresa%TYPE,
    descripcion pasantia.descripcion%TYPE,
    programa pasantia.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.pais,
        h.empresa,
        h.descripcion,
        h.programa,
        h.activo,
        p.nombre
    FROM pasantia h
    JOIN programa p ON p.id = h.programa
    WHERE h.id = p_id AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_pasantia(
    p_id pasantia.id%TYPE,
    p_nombre pasantia.nombre%TYPE,
    p_pais pasantia.pais%TYPE,
    p_empresa pasantia.empresa%TYPE,
    p_descripcion pasantia.descripcion%TYPE,
    p_programa pasantia.programa%TYPE
)
RETURNS pasantia
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila pasantia;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'pasantia', COLUMN = 'programa';
    END IF;
    INSERT INTO pasantia (id, nombre, pais, empresa, descripcion, programa, activo)
    VALUES (p_id, p_nombre, p_pais, p_empresa, p_descripcion, p_programa, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_pasantia(
    p_id pasantia.id%TYPE,
    p_nombre pasantia.nombre%TYPE,
    p_pais pasantia.pais%TYPE,
    p_empresa pasantia.empresa%TYPE,
    p_descripcion pasantia.descripcion%TYPE,
    p_programa pasantia.programa%TYPE
)
RETURNS SETOF pasantia
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila pasantia;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'pasantia', COLUMN = 'programa';
    END IF;
    UPDATE pasantia SET
        nombre = p_nombre,
        pais = p_pais,
        empresa = p_empresa,
        descripcion = p_descripcion,
        programa = p_programa
    WHERE id = p_id AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_pasantia(p_id pasantia.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE pasantia SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- premio (premios)
-- ============================================================

CREATE OR REPLACE FUNCTION sp_listar_premio()
RETURNS TABLE (
    id premio.id%TYPE,
    nombre premio.nombre%TYPE,
    descripcion premio.descripcion%TYPE,
    fecha premio.fecha%TYPE,
    entidad_otorgante premio.entidad_otorgante%TYPE,
    pais premio.pais%TYPE,
    programa premio.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.descripcion,
        h.fecha,
        h.entidad_otorgante,
        h.pais,
        h.programa,
        h.activo,
        p.nombre
    FROM premio h
    JOIN programa p ON p.id = h.programa
    WHERE h.activo
    ORDER BY h.id;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_premio(p_id premio.id%TYPE)
RETURNS TABLE (
    id premio.id%TYPE,
    nombre premio.nombre%TYPE,
    descripcion premio.descripcion%TYPE,
    fecha premio.fecha%TYPE,
    entidad_otorgante premio.entidad_otorgante%TYPE,
    pais premio.pais%TYPE,
    programa premio.programa%TYPE,
    activo BOOLEAN,
    programa_nombre programa.nombre%TYPE
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.id,
        h.nombre,
        h.descripcion,
        h.fecha,
        h.entidad_otorgante,
        h.pais,
        h.programa,
        h.activo,
        p.nombre
    FROM premio h
    JOIN programa p ON p.id = h.programa
    WHERE h.id = p_id AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_premio(
    p_id premio.id%TYPE,
    p_nombre premio.nombre%TYPE,
    p_descripcion premio.descripcion%TYPE,
    p_fecha premio.fecha%TYPE,
    p_entidad_otorgante premio.entidad_otorgante%TYPE,
    p_pais premio.pais%TYPE,
    p_programa premio.programa%TYPE
)
RETURNS premio
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila premio;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'premio', COLUMN = 'programa';
    END IF;
    INSERT INTO premio (id, nombre, descripcion, fecha, entidad_otorgante, pais, programa, activo)
    VALUES (p_id, p_nombre, p_descripcion, p_fecha, p_entidad_otorgante, p_pais, p_programa, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_premio(
    p_id premio.id%TYPE,
    p_nombre premio.nombre%TYPE,
    p_descripcion premio.descripcion%TYPE,
    p_fecha premio.fecha%TYPE,
    p_entidad_otorgante premio.entidad_otorgante%TYPE,
    p_pais premio.pais%TYPE,
    p_programa premio.programa%TYPE
)
RETURNS SETOF premio
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila premio;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'premio', COLUMN = 'programa';
    END IF;
    UPDATE premio SET
        nombre = p_nombre,
        descripcion = p_descripcion,
        fecha = p_fecha,
        entidad_otorgante = p_entidad_otorgante,
        pais = p_pais,
        programa = p_programa
    WHERE id = p_id AND activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN NEXT v_fila;
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_premio(p_id premio.id%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE premio SET activo = FALSE WHERE id = p_id AND activo;
    RETURN FOUND;
END;
$$;
