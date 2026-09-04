-- Docker ejecuta este archivo DESPUÉS de 00_innovacion_curricular.pg.sql
-- (por orden alfabético), así que las 22 tablas ya existen cuando corre esto.
-- Punto 1.5 de la constitución: el script del profesor no trae borrado
-- lógico en estas tablas, así que lo agregamos aquí, una sola vez.

ALTER TABLE area_conocimiento    ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE universidad          ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE aspecto_normativo    ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE practica_estrategia  ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE enfoque              ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE car_innovacion       ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE aliado               ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE facultad             ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE programa             ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE acreditacion         ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE registro_calificado  ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE activ_academica      ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE pasantia             ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE premio               ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE programa_ac          ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE programa_pe          ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE programa_ci          ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE an_programa          ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE enfoque_rc           ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE aa_rc                ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE docente_departamento ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
ALTER TABLE alianza              ADD COLUMN IF NOT EXISTS activo BOOLEAN DEFAULT TRUE;
