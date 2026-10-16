-- Docker ejecuta este archivo DESPUÉS de 04_sp_v2.sql (orden alfabético: el punto
-- va antes que el guion bajo). Se separó en dos archivos solo por tamaño: este
-- trae lo que no es una entidad de llave simple.
--
--  1. Las 8 tablas puente: asignar (crear), retirar (eliminar), listar y
--     consultar con la llave compuesta. Las 5 de solo llaves (programa_ac,
--     programa_pe, programa_ci, an_programa, enfoque_rc) no tienen actualizar:
--     la pareja existe o no existe (RF6). Las 3 con campos propios (aa_rc,
--     docente_departamento, alianza) tienen los 5 verbos.
--  2. Los 7 lotes del maestro-detalle: varios renglones en UNA función = UNA
--     transacción; si falla uno no se guarda ninguno (RF8, D-v2-5).
--  3. Los 3 conteos que usa la regla de retiro de un maestro (RF12, D-v2-4).
--
-- Reglas de los puentes:
--  * Asignar comprueba que los dos lados existan Y estén activos (23503, igual
--    que en 04_sp_v2.sql). docente_departamento.docente NO tiene llave foránea
--    porque su tabla es de otro módulo (VERSION_2.md).
--  * Si la pareja existe RETIRADA, asignar la reactiva y devuelve la fila (B6,
--    D-v2-7). Si existe ACTIVA, el INSERT falla con el 23505 nativo de la llave
--    primaria ({tabla}_pkey), que la API traduce a 409.
--  * Retirar es activo = FALSE; devuelve TRUE si retiró algo.

-- ============================================================
-- 1. TABLAS PUENTE
-- ============================================================

-- programa_ac

CREATE OR REPLACE FUNCTION sp_listar_programa_ac()
RETURNS TABLE (
    programa programa_ac.programa%TYPE,
    area_conocimiento programa_ac.area_conocimiento%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    area_conocimiento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.area_conocimiento,
        h.activo,
        p1.nombre,
        p2.disciplina || ' (' || p2.area || ')'
    FROM programa_ac h
    JOIN programa p1 ON p1.id = h.programa
    JOIN area_conocimiento p2 ON p2.id = h.area_conocimiento
    WHERE h.activo
    ORDER BY h.programa, h.area_conocimiento;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_programa_ac(p_programa programa_ac.programa%TYPE, p_area_conocimiento programa_ac.area_conocimiento%TYPE)
RETURNS TABLE (
    programa programa_ac.programa%TYPE,
    area_conocimiento programa_ac.area_conocimiento%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    area_conocimiento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.area_conocimiento,
        h.activo,
        p1.nombre,
        p2.disciplina || ' (' || p2.area || ')'
    FROM programa_ac h
    JOIN programa p1 ON p1.id = h.programa
    JOIN area_conocimiento p2 ON p2.id = h.area_conocimiento
    WHERE h.programa = p_programa AND h.area_conocimiento = p_area_conocimiento AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_programa_ac(
    p_programa programa_ac.programa%TYPE,
    p_area_conocimiento programa_ac.area_conocimiento%TYPE
)
RETURNS programa_ac
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila programa_ac;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_ac', COLUMN = 'programa';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM area_conocimiento WHERE id = p_area_conocimiento AND activo) THEN
        RAISE EXCEPTION 'el campo area_conocimiento no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_ac', COLUMN = 'area_conocimiento';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE programa_ac SET activo = TRUE
    WHERE programa = p_programa AND area_conocimiento = p_area_conocimiento AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO programa_ac (programa, area_conocimiento, activo)
    VALUES (p_programa, p_area_conocimiento, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_programa_ac(p_programa programa_ac.programa%TYPE, p_area_conocimiento programa_ac.area_conocimiento%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE programa_ac SET activo = FALSE WHERE programa = p_programa AND area_conocimiento = p_area_conocimiento AND activo;
    RETURN FOUND;
END;
$$;

-- programa_pe

CREATE OR REPLACE FUNCTION sp_listar_programa_pe()
RETURNS TABLE (
    programa programa_pe.programa%TYPE,
    practica_estrategia programa_pe.practica_estrategia%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    practica_estrategia_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.practica_estrategia,
        h.activo,
        p1.nombre,
        p2.nombre
    FROM programa_pe h
    JOIN programa p1 ON p1.id = h.programa
    JOIN practica_estrategia p2 ON p2.id = h.practica_estrategia
    WHERE h.activo
    ORDER BY h.programa, h.practica_estrategia;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_programa_pe(p_programa programa_pe.programa%TYPE, p_practica_estrategia programa_pe.practica_estrategia%TYPE)
RETURNS TABLE (
    programa programa_pe.programa%TYPE,
    practica_estrategia programa_pe.practica_estrategia%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    practica_estrategia_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.practica_estrategia,
        h.activo,
        p1.nombre,
        p2.nombre
    FROM programa_pe h
    JOIN programa p1 ON p1.id = h.programa
    JOIN practica_estrategia p2 ON p2.id = h.practica_estrategia
    WHERE h.programa = p_programa AND h.practica_estrategia = p_practica_estrategia AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_programa_pe(
    p_programa programa_pe.programa%TYPE,
    p_practica_estrategia programa_pe.practica_estrategia%TYPE
)
RETURNS programa_pe
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila programa_pe;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_pe', COLUMN = 'programa';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM practica_estrategia WHERE id = p_practica_estrategia AND activo) THEN
        RAISE EXCEPTION 'el campo practica_estrategia no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_pe', COLUMN = 'practica_estrategia';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE programa_pe SET activo = TRUE
    WHERE programa = p_programa AND practica_estrategia = p_practica_estrategia AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO programa_pe (programa, practica_estrategia, activo)
    VALUES (p_programa, p_practica_estrategia, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_programa_pe(p_programa programa_pe.programa%TYPE, p_practica_estrategia programa_pe.practica_estrategia%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE programa_pe SET activo = FALSE WHERE programa = p_programa AND practica_estrategia = p_practica_estrategia AND activo;
    RETURN FOUND;
END;
$$;

-- programa_ci

CREATE OR REPLACE FUNCTION sp_listar_programa_ci()
RETURNS TABLE (
    programa programa_ci.programa%TYPE,
    car_innovacion programa_ci.car_innovacion%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    car_innovacion_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.car_innovacion,
        h.activo,
        p1.nombre,
        p2.nombre
    FROM programa_ci h
    JOIN programa p1 ON p1.id = h.programa
    JOIN car_innovacion p2 ON p2.id = h.car_innovacion
    WHERE h.activo
    ORDER BY h.programa, h.car_innovacion;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_programa_ci(p_programa programa_ci.programa%TYPE, p_car_innovacion programa_ci.car_innovacion%TYPE)
RETURNS TABLE (
    programa programa_ci.programa%TYPE,
    car_innovacion programa_ci.car_innovacion%TYPE,
    activo BOOLEAN,
    programa_nombre TEXT,
    car_innovacion_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.programa,
        h.car_innovacion,
        h.activo,
        p1.nombre,
        p2.nombre
    FROM programa_ci h
    JOIN programa p1 ON p1.id = h.programa
    JOIN car_innovacion p2 ON p2.id = h.car_innovacion
    WHERE h.programa = p_programa AND h.car_innovacion = p_car_innovacion AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_programa_ci(
    p_programa programa_ci.programa%TYPE,
    p_car_innovacion programa_ci.car_innovacion%TYPE
)
RETURNS programa_ci
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila programa_ci;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_ci', COLUMN = 'programa';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM car_innovacion WHERE id = p_car_innovacion AND activo) THEN
        RAISE EXCEPTION 'el campo car_innovacion no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa_ci', COLUMN = 'car_innovacion';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE programa_ci SET activo = TRUE
    WHERE programa = p_programa AND car_innovacion = p_car_innovacion AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO programa_ci (programa, car_innovacion, activo)
    VALUES (p_programa, p_car_innovacion, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_programa_ci(p_programa programa_ci.programa%TYPE, p_car_innovacion programa_ci.car_innovacion%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE programa_ci SET activo = FALSE WHERE programa = p_programa AND car_innovacion = p_car_innovacion AND activo;
    RETURN FOUND;
END;
$$;

-- an_programa

CREATE OR REPLACE FUNCTION sp_listar_an_programa()
RETURNS TABLE (
    aspecto_normativo an_programa.aspecto_normativo%TYPE,
    programa an_programa.programa%TYPE,
    activo BOOLEAN,
    aspecto_normativo_nombre TEXT,
    programa_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.aspecto_normativo,
        h.programa,
        h.activo,
        p1.tipo || ' — ' || p1.descripcion,
        p2.nombre
    FROM an_programa h
    JOIN aspecto_normativo p1 ON p1.id = h.aspecto_normativo
    JOIN programa p2 ON p2.id = h.programa
    WHERE h.activo
    ORDER BY h.aspecto_normativo, h.programa;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_an_programa(p_aspecto_normativo an_programa.aspecto_normativo%TYPE, p_programa an_programa.programa%TYPE)
RETURNS TABLE (
    aspecto_normativo an_programa.aspecto_normativo%TYPE,
    programa an_programa.programa%TYPE,
    activo BOOLEAN,
    aspecto_normativo_nombre TEXT,
    programa_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.aspecto_normativo,
        h.programa,
        h.activo,
        p1.tipo || ' — ' || p1.descripcion,
        p2.nombre
    FROM an_programa h
    JOIN aspecto_normativo p1 ON p1.id = h.aspecto_normativo
    JOIN programa p2 ON p2.id = h.programa
    WHERE h.aspecto_normativo = p_aspecto_normativo AND h.programa = p_programa AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_an_programa(
    p_aspecto_normativo an_programa.aspecto_normativo%TYPE,
    p_programa an_programa.programa%TYPE
)
RETURNS an_programa
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila an_programa;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM aspecto_normativo WHERE id = p_aspecto_normativo AND activo) THEN
        RAISE EXCEPTION 'el campo aspecto_normativo no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'an_programa', COLUMN = 'aspecto_normativo';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_programa AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'an_programa', COLUMN = 'programa';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE an_programa SET activo = TRUE
    WHERE aspecto_normativo = p_aspecto_normativo AND programa = p_programa AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO an_programa (aspecto_normativo, programa, activo)
    VALUES (p_aspecto_normativo, p_programa, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_an_programa(p_aspecto_normativo an_programa.aspecto_normativo%TYPE, p_programa an_programa.programa%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE an_programa SET activo = FALSE WHERE aspecto_normativo = p_aspecto_normativo AND programa = p_programa AND activo;
    RETURN FOUND;
END;
$$;

-- enfoque_rc

CREATE OR REPLACE FUNCTION sp_listar_enfoque_rc()
RETURNS TABLE (
    enfoque enfoque_rc.enfoque%TYPE,
    registro_calificado enfoque_rc.registro_calificado%TYPE,
    activo BOOLEAN,
    enfoque_nombre TEXT,
    registro_calificado_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.enfoque,
        h.registro_calificado,
        h.activo,
        p1.nombre,
        p2.codigo::text
    FROM enfoque_rc h
    JOIN enfoque p1 ON p1.id = h.enfoque
    JOIN registro_calificado p2 ON p2.codigo = h.registro_calificado
    WHERE h.activo
    ORDER BY h.enfoque, h.registro_calificado;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_enfoque_rc(p_enfoque enfoque_rc.enfoque%TYPE, p_registro_calificado enfoque_rc.registro_calificado%TYPE)
RETURNS TABLE (
    enfoque enfoque_rc.enfoque%TYPE,
    registro_calificado enfoque_rc.registro_calificado%TYPE,
    activo BOOLEAN,
    enfoque_nombre TEXT,
    registro_calificado_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.enfoque,
        h.registro_calificado,
        h.activo,
        p1.nombre,
        p2.codigo::text
    FROM enfoque_rc h
    JOIN enfoque p1 ON p1.id = h.enfoque
    JOIN registro_calificado p2 ON p2.codigo = h.registro_calificado
    WHERE h.enfoque = p_enfoque AND h.registro_calificado = p_registro_calificado AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_enfoque_rc(
    p_enfoque enfoque_rc.enfoque%TYPE,
    p_registro_calificado enfoque_rc.registro_calificado%TYPE
)
RETURNS enfoque_rc
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila enfoque_rc;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM enfoque WHERE id = p_enfoque AND activo) THEN
        RAISE EXCEPTION 'el campo enfoque no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'enfoque_rc', COLUMN = 'enfoque';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM registro_calificado WHERE codigo = p_registro_calificado AND activo) THEN
        RAISE EXCEPTION 'el campo registro_calificado no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'enfoque_rc', COLUMN = 'registro_calificado';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE enfoque_rc SET activo = TRUE
    WHERE enfoque = p_enfoque AND registro_calificado = p_registro_calificado AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO enfoque_rc (enfoque, registro_calificado, activo)
    VALUES (p_enfoque, p_registro_calificado, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_enfoque_rc(p_enfoque enfoque_rc.enfoque%TYPE, p_registro_calificado enfoque_rc.registro_calificado%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE enfoque_rc SET activo = FALSE WHERE enfoque = p_enfoque AND registro_calificado = p_registro_calificado AND activo;
    RETURN FOUND;
END;
$$;

-- aa_rc

CREATE OR REPLACE FUNCTION sp_listar_aa_rc()
RETURNS TABLE (
    activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE,
    registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE,
    componente aa_rc.componente%TYPE,
    semestre aa_rc.semestre%TYPE,
    activo BOOLEAN,
    activ_academica_nombre TEXT,
    registro_calificado_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.activ_academicas_idcurso,
        h.registro_calificado_codigo,
        h.componente,
        h.semestre,
        h.activo,
        p1.nombre,
        p2.codigo::text
    FROM aa_rc h
    JOIN activ_academica p1 ON p1.id = h.activ_academicas_idcurso
    JOIN registro_calificado p2 ON p2.codigo = h.registro_calificado_codigo
    WHERE h.activo
    ORDER BY h.activ_academicas_idcurso, h.registro_calificado_codigo;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_aa_rc(p_activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE, p_registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE)
RETURNS TABLE (
    activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE,
    registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE,
    componente aa_rc.componente%TYPE,
    semestre aa_rc.semestre%TYPE,
    activo BOOLEAN,
    activ_academica_nombre TEXT,
    registro_calificado_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.activ_academicas_idcurso,
        h.registro_calificado_codigo,
        h.componente,
        h.semestre,
        h.activo,
        p1.nombre,
        p2.codigo::text
    FROM aa_rc h
    JOIN activ_academica p1 ON p1.id = h.activ_academicas_idcurso
    JOIN registro_calificado p2 ON p2.codigo = h.registro_calificado_codigo
    WHERE h.activ_academicas_idcurso = p_activ_academicas_idcurso AND h.registro_calificado_codigo = p_registro_calificado_codigo AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_aa_rc(
    p_activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE,
    p_registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE,
    p_componente aa_rc.componente%TYPE,
    p_semestre aa_rc.semestre%TYPE
)
RETURNS aa_rc
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila aa_rc;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM activ_academica WHERE id = p_activ_academicas_idcurso AND activo) THEN
        RAISE EXCEPTION 'el campo activ_academicas_idcurso no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'aa_rc', COLUMN = 'activ_academicas_idcurso';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM registro_calificado WHERE codigo = p_registro_calificado_codigo AND activo) THEN
        RAISE EXCEPTION 'el campo registro_calificado_codigo no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'aa_rc', COLUMN = 'registro_calificado_codigo';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE aa_rc SET activo = TRUE, componente = p_componente, semestre = p_semestre
    WHERE activ_academicas_idcurso = p_activ_academicas_idcurso AND registro_calificado_codigo = p_registro_calificado_codigo AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO aa_rc (activ_academicas_idcurso, registro_calificado_codigo, componente, semestre, activo)
    VALUES (p_activ_academicas_idcurso, p_registro_calificado_codigo, p_componente, p_semestre, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_aa_rc(
    p_activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE,
    p_registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE,
    p_componente aa_rc.componente%TYPE,
    p_semestre aa_rc.semestre%TYPE
)
RETURNS SETOF aa_rc
LANGUAGE sql
AS $$
    UPDATE aa_rc SET
        componente = p_componente,
        semestre = p_semestre
    WHERE activ_academicas_idcurso = p_activ_academicas_idcurso AND registro_calificado_codigo = p_registro_calificado_codigo AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_aa_rc(p_activ_academicas_idcurso aa_rc.activ_academicas_idcurso%TYPE, p_registro_calificado_codigo aa_rc.registro_calificado_codigo%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE aa_rc SET activo = FALSE WHERE activ_academicas_idcurso = p_activ_academicas_idcurso AND registro_calificado_codigo = p_registro_calificado_codigo AND activo;
    RETURN FOUND;
END;
$$;

-- docente_departamento

CREATE OR REPLACE FUNCTION sp_listar_docente_departamento()
RETURNS TABLE (
    docente docente_departamento.docente%TYPE,
    departamento docente_departamento.departamento%TYPE,
    dedicacion docente_departamento.dedicacion%TYPE,
    modalidad docente_departamento.modalidad%TYPE,
    fecha_ingreso docente_departamento.fecha_ingreso%TYPE,
    fecha_salida docente_departamento.fecha_salida%TYPE,
    activo BOOLEAN,
    departamento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.docente,
        h.departamento,
        h.dedicacion,
        h.modalidad,
        h.fecha_ingreso,
        h.fecha_salida,
        h.activo,
        p1.nombre
    FROM docente_departamento h
    JOIN programa p1 ON p1.id = h.departamento
    WHERE h.activo
    ORDER BY h.docente, h.departamento;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_docente_departamento(p_docente docente_departamento.docente%TYPE, p_departamento docente_departamento.departamento%TYPE)
RETURNS TABLE (
    docente docente_departamento.docente%TYPE,
    departamento docente_departamento.departamento%TYPE,
    dedicacion docente_departamento.dedicacion%TYPE,
    modalidad docente_departamento.modalidad%TYPE,
    fecha_ingreso docente_departamento.fecha_ingreso%TYPE,
    fecha_salida docente_departamento.fecha_salida%TYPE,
    activo BOOLEAN,
    departamento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.docente,
        h.departamento,
        h.dedicacion,
        h.modalidad,
        h.fecha_ingreso,
        h.fecha_salida,
        h.activo,
        p1.nombre
    FROM docente_departamento h
    JOIN programa p1 ON p1.id = h.departamento
    WHERE h.docente = p_docente AND h.departamento = p_departamento AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_docente_departamento(
    p_docente docente_departamento.docente%TYPE,
    p_departamento docente_departamento.departamento%TYPE,
    p_dedicacion docente_departamento.dedicacion%TYPE,
    p_modalidad docente_departamento.modalidad%TYPE,
    p_fecha_ingreso docente_departamento.fecha_ingreso%TYPE,
    p_fecha_salida docente_departamento.fecha_salida%TYPE
)
RETURNS docente_departamento
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila docente_departamento;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_departamento AND activo) THEN
        RAISE EXCEPTION 'el campo departamento no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'docente_departamento', COLUMN = 'departamento';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE docente_departamento SET activo = TRUE, dedicacion = p_dedicacion, modalidad = p_modalidad, fecha_ingreso = p_fecha_ingreso, fecha_salida = p_fecha_salida
    WHERE docente = p_docente AND departamento = p_departamento AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO docente_departamento (docente, departamento, dedicacion, modalidad, fecha_ingreso, fecha_salida, activo)
    VALUES (p_docente, p_departamento, p_dedicacion, p_modalidad, p_fecha_ingreso, p_fecha_salida, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_docente_departamento(
    p_docente docente_departamento.docente%TYPE,
    p_departamento docente_departamento.departamento%TYPE,
    p_dedicacion docente_departamento.dedicacion%TYPE,
    p_modalidad docente_departamento.modalidad%TYPE,
    p_fecha_ingreso docente_departamento.fecha_ingreso%TYPE,
    p_fecha_salida docente_departamento.fecha_salida%TYPE
)
RETURNS SETOF docente_departamento
LANGUAGE sql
AS $$
    UPDATE docente_departamento SET
        dedicacion = p_dedicacion,
        modalidad = p_modalidad,
        fecha_ingreso = p_fecha_ingreso,
        fecha_salida = p_fecha_salida
    WHERE docente = p_docente AND departamento = p_departamento AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_docente_departamento(p_docente docente_departamento.docente%TYPE, p_departamento docente_departamento.departamento%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE docente_departamento SET activo = FALSE WHERE docente = p_docente AND departamento = p_departamento AND activo;
    RETURN FOUND;
END;
$$;

-- alianza

CREATE OR REPLACE FUNCTION sp_listar_alianza()
RETURNS TABLE (
    aliado alianza.aliado%TYPE,
    departamento alianza.departamento%TYPE,
    fecha_inicio alianza.fecha_inicio%TYPE,
    fecha_fin alianza.fecha_fin%TYPE,
    docente alianza.docente%TYPE,
    activo BOOLEAN,
    aliado_nombre TEXT,
    departamento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.aliado,
        h.departamento,
        h.fecha_inicio,
        h.fecha_fin,
        h.docente,
        h.activo,
        p1.razon_social,
        p2.nombre
    FROM alianza h
    JOIN aliado p1 ON p1.nit = h.aliado
    JOIN programa p2 ON p2.id = h.departamento
    WHERE h.activo
    ORDER BY h.aliado, h.departamento;
$$;

CREATE OR REPLACE FUNCTION sp_consultar_alianza(p_aliado alianza.aliado%TYPE, p_departamento alianza.departamento%TYPE)
RETURNS TABLE (
    aliado alianza.aliado%TYPE,
    departamento alianza.departamento%TYPE,
    fecha_inicio alianza.fecha_inicio%TYPE,
    fecha_fin alianza.fecha_fin%TYPE,
    docente alianza.docente%TYPE,
    activo BOOLEAN,
    aliado_nombre TEXT,
    departamento_nombre TEXT
)
LANGUAGE sql STABLE
AS $$
    SELECT
        h.aliado,
        h.departamento,
        h.fecha_inicio,
        h.fecha_fin,
        h.docente,
        h.activo,
        p1.razon_social,
        p2.nombre
    FROM alianza h
    JOIN aliado p1 ON p1.nit = h.aliado
    JOIN programa p2 ON p2.id = h.departamento
    WHERE h.aliado = p_aliado AND h.departamento = p_departamento AND h.activo;
$$;

CREATE OR REPLACE FUNCTION sp_crear_alianza(
    p_aliado alianza.aliado%TYPE,
    p_departamento alianza.departamento%TYPE,
    p_fecha_inicio alianza.fecha_inicio%TYPE,
    p_fecha_fin alianza.fecha_fin%TYPE,
    p_docente alianza.docente%TYPE
)
RETURNS alianza
LANGUAGE plpgsql
AS $$
DECLARE
    v_fila alianza;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM aliado WHERE nit = p_aliado AND activo) THEN
        RAISE EXCEPTION 'el campo aliado no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'alianza', COLUMN = 'aliado';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_departamento AND activo) THEN
        RAISE EXCEPTION 'el campo departamento no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'alianza', COLUMN = 'departamento';
    END IF;
    -- Pareja retirada: se reactiva en vez de chocar con la llave primaria (B6).
    UPDATE alianza SET activo = TRUE, fecha_inicio = p_fecha_inicio, fecha_fin = p_fecha_fin, docente = p_docente
    WHERE aliado = p_aliado AND departamento = p_departamento AND NOT activo
    RETURNING * INTO v_fila;
    IF FOUND THEN
        RETURN v_fila;
    END IF;
    INSERT INTO alianza (aliado, departamento, fecha_inicio, fecha_fin, docente, activo)
    VALUES (p_aliado, p_departamento, p_fecha_inicio, p_fecha_fin, p_docente, TRUE)
    RETURNING * INTO v_fila;
    RETURN v_fila;
END;
$$;

CREATE OR REPLACE FUNCTION sp_actualizar_alianza(
    p_aliado alianza.aliado%TYPE,
    p_departamento alianza.departamento%TYPE,
    p_fecha_inicio alianza.fecha_inicio%TYPE,
    p_fecha_fin alianza.fecha_fin%TYPE,
    p_docente alianza.docente%TYPE
)
RETURNS SETOF alianza
LANGUAGE sql
AS $$
    UPDATE alianza SET
        fecha_inicio = p_fecha_inicio,
        fecha_fin = p_fecha_fin,
        docente = p_docente
    WHERE aliado = p_aliado AND departamento = p_departamento AND activo
    RETURNING *;
$$;

CREATE OR REPLACE FUNCTION sp_eliminar_alianza(p_aliado alianza.aliado%TYPE, p_departamento alianza.departamento%TYPE)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE alianza SET activo = FALSE WHERE aliado = p_aliado AND departamento = p_departamento AND activo;
    RETURN FOUND;
END;
$$;

-- ============================================================
-- 2. LOTES DEL MAESTRO-DETALLE
-- ============================================================
-- Contrato común: sp_crear_<detalle>_lote(p_maestro, p_renglones jsonb).
--  * El maestro viaja aparte; cada renglón trae solo los campos del detalle.
--  * Las claves del JSON se llaman como las COLUMNAS (fecha_inicio, no
--    fechaInicio): el repositorio de C# serializa en snake_case.
--  * Se valida el maestro una vez (existe y está activo) y luego se inserta
--    renglón por renglón. Si un renglón repite una llave, se lanza un error que
--    nombra el renglón («renglón 2: ...»); como todo ocurre dentro de UNA función
--    (una transacción), no queda guardado ninguno.
--  * Devuelve las filas como quedaron guardadas.

CREATE OR REPLACE FUNCTION sp_crear_facultad_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF facultad
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila facultad;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM universidad WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo universidad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'facultad', COLUMN = 'universidad';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(id INT, nombre VARCHAR, tipo VARCHAR, fecha_fun DATE)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO facultad (id, nombre, tipo, fecha_fun, universidad, activo)
            VALUES (r.id, r.nombre, r.tipo, r.fecha_fun, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe una facultad con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'facultad', CONSTRAINT = 'facultad_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_programa_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF programa
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila programa;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM facultad WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo facultad no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'programa', COLUMN = 'facultad';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(id INT, nombre VARCHAR, tipo VARCHAR, nivel VARCHAR, fecha_creacion VARCHAR, fecha_cierre VARCHAR, numero_cohortes VARCHAR, cant_graduados VARCHAR, fecha_actualizacion VARCHAR, ciudad VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO programa (id, nombre, tipo, nivel, fecha_creacion, fecha_cierre, numero_cohortes, cant_graduados, fecha_actualizacion, ciudad, facultad, activo)
            VALUES (r.id, r.nombre, r.tipo, r.nivel, r.fecha_creacion, r.fecha_cierre, r.numero_cohortes, r.cant_graduados, r.fecha_actualizacion, r.ciudad, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe un programa con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'programa', CONSTRAINT = 'programa_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_acreditacion_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF acreditacion
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila acreditacion;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'acreditacion', COLUMN = 'programa';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(resolucion INT, tipo VARCHAR, calificacion VARCHAR, fecha_inicio VARCHAR, fecha_fin VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO acreditacion (resolucion, tipo, calificacion, fecha_inicio, fecha_fin, programa, activo)
            VALUES (r.resolucion, r.tipo, r.calificacion, r.fecha_inicio, r.fecha_fin, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe una acreditación con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'acreditacion', CONSTRAINT = 'acreditacion_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_registro_calificado_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF registro_calificado
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila registro_calificado;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'registro_calificado', COLUMN = 'programa';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(codigo INT, cant_creditos VARCHAR, hora_acom VARCHAR, hora_ind VARCHAR, metodologia VARCHAR, fecha_inicio DATE, fecha_fin DATE, duracion_anios VARCHAR, duracion_semestres VARCHAR, tipo_titulacion VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO registro_calificado (codigo, cant_creditos, hora_acom, hora_ind, metodologia, fecha_inicio, fecha_fin, duracion_anios, duracion_semestres, tipo_titulacion, programa, activo)
            VALUES (r.codigo, r.cant_creditos, r.hora_acom, r.hora_ind, r.metodologia, r.fecha_inicio, r.fecha_fin, r.duracion_anios, r.duracion_semestres, r.tipo_titulacion, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe un registro calificado con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'registro_calificado', CONSTRAINT = 'registro_calificado_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_activ_academica_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF activ_academica
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila activ_academica;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo disenio no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'activ_academica', COLUMN = 'disenio';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(id INT, nombre VARCHAR, num_creditos INT, tipo VARCHAR, area_formacion VARCHAR, h_acom INT, h_indep INT, idioma VARCHAR, espejo INT, entidad_espejo VARCHAR, pais_espejo VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO activ_academica (id, nombre, num_creditos, tipo, area_formacion, h_acom, h_indep, idioma, espejo, entidad_espejo, pais_espejo, disenio, activo)
            VALUES (r.id, r.nombre, r.num_creditos, r.tipo, r.area_formacion, r.h_acom, r.h_indep, r.idioma, r.espejo, r.entidad_espejo, r.pais_espejo, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe una actividad académica con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'activ_academica', CONSTRAINT = 'activ_academica_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_pasantia_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF pasantia
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila pasantia;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'pasantia', COLUMN = 'programa';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(id INT, nombre VARCHAR, pais VARCHAR, empresa VARCHAR, descripcion VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO pasantia (id, nombre, pais, empresa, descripcion, programa, activo)
            VALUES (r.id, r.nombre, r.pais, r.empresa, r.descripcion, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe una pasantía con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'pasantia', CONSTRAINT = 'pasantia_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

CREATE OR REPLACE FUNCTION sp_crear_premio_lote(p_maestro INT, p_renglones JSONB)
RETURNS SETOF premio
LANGUAGE plpgsql
AS $$
DECLARE
    r RECORD;
    v_fila premio;
    v_n INT := 0;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM programa WHERE id = p_maestro AND activo) THEN
        RAISE EXCEPTION 'el campo programa no corresponde a un registro existente y activo'
            USING ERRCODE = '23503', TABLE = 'premio', COLUMN = 'programa';
    END IF;

    FOR r IN SELECT * FROM jsonb_to_recordset(p_renglones) AS x(id INT, nombre VARCHAR, descripcion VARCHAR, fecha DATE, entidad_otorgante VARCHAR, pais VARCHAR)
    LOOP
        v_n := v_n + 1;
        BEGIN
            INSERT INTO premio (id, nombre, descripcion, fecha, entidad_otorgante, pais, programa, activo)
            VALUES (r.id, r.nombre, r.descripcion, r.fecha, r.entidad_otorgante, r.pais, p_maestro, TRUE)
            RETURNING * INTO v_fila;
        EXCEPTION WHEN unique_violation THEN
            RAISE EXCEPTION 'renglón %: ya existe un premio con esa llave', v_n
                USING ERRCODE = '23505', TABLE = 'premio', CONSTRAINT = 'premio_pkey';
        END;
        RETURN NEXT v_fila;
    END LOOP;
END;
$$;

-- ============================================================
-- 3. CONTEOS PARA LA REGLA DE RETIRO (B1)
-- ============================================================
-- El servicio los llama antes de retirar un maestro: si devuelven más de cero,
-- responde 409 «tiene registros activos». La regla vive en el servicio y no en
-- sp_eliminar_* (D-v2-4): estos conteos son solo la pregunta.

CREATE OR REPLACE FUNCTION sp_contar_detalle_universidad(p_id universidad.id%TYPE)
RETURNS INT
LANGUAGE sql STABLE
AS $$
    SELECT count(*)::INT FROM facultad WHERE universidad = p_id AND activo;
$$;

CREATE OR REPLACE FUNCTION sp_contar_detalle_facultad(p_id facultad.id%TYPE)
RETURNS INT
LANGUAGE sql STABLE
AS $$
    SELECT count(*)::INT FROM programa WHERE facultad = p_id AND activo;
$$;

-- El detalle de un programa son sus cinco tablas hijas (VERSION_2.md §3).
CREATE OR REPLACE FUNCTION sp_contar_detalle_programa(p_id programa.id%TYPE)
RETURNS INT
LANGUAGE sql STABLE
AS $$
    SELECT (SELECT count(*) FROM acreditacion        WHERE programa = p_id AND activo)
         + (SELECT count(*) FROM registro_calificado WHERE programa = p_id AND activo)
         + (SELECT count(*) FROM activ_academica     WHERE disenio  = p_id AND activo)
         + (SELECT count(*) FROM pasantia            WHERE programa = p_id AND activo)
         + (SELECT count(*) FROM premio              WHERE programa = p_id AND activo);
$$;