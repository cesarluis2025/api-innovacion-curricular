-- Docker ejecuta este archivo DESPUÉS de 01_alter_activo.sql (orden alfabético).
-- Cambios de columnas que la v2 necesita y que el script del profesor no trae
-- (5_data_model.md, "Lo que la v2 cambia en tablas de la v1").
-- Es idempotente: se puede volver a correr sin error.

-- Contador de programas por facultad. NUNCA lo escribe la API: lo mantiene el
-- disparador de 05_disparadores.sql, para que el dato derivado no dependa de
-- que cada pantalla se acuerde de actualizarlo (RF10).
ALTER TABLE facultad
    ADD COLUMN IF NOT EXISTS total_programas INT NOT NULL DEFAULT 0;

-- El Excel de datos del profesor trae 19 disciplinas de más de 60 caracteres;
-- con VARCHAR(60) la carga de referencia (fase 6) fallaría (D-v2-12).
ALTER TABLE area_conocimiento
    ALTER COLUMN disciplina TYPE VARCHAR(130);

-- Igual con 25 nombres de programa que pasan de 60 caracteres.
ALTER TABLE programa
    ALTER COLUMN nombre TYPE VARCHAR(100);