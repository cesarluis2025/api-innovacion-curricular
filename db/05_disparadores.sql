-- Disparador de la v2 (RF10): facultad.total_programas se mantiene SOLO.
--
-- Por qué un disparador y no un cálculo en la API: el contador debe ser cierto
-- venga el cambio de donde venga (la API, psql, un script de carga). Si lo
-- calculara el servicio, cualquier INSERT directo lo dejaría desfasado.
--
-- Por qué se recuenta (COUNT) y no se suma/resta 1: sumar y restar se
-- descuadra con un error a medias; contar siempre deja el valor correcto y
-- vuelve a ser cierto en el siguiente cambio.
--
-- Por qué se recalculan OLD y NEW: al mover un programa de la facultad 1 a la 2
-- hay que bajar la 1 y subir la 2. Recalcular solo NEW dejaría mal la vieja.
-- Cuenta solo programas activos, de modo que retirarlo (activo = FALSE) lo baja
-- y reactivarlo lo sube.
-- CREATE OR REPLACE: se puede volver a aplicar sin recrear la base.

CREATE OR REPLACE FUNCTION fn_contar_programas()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- NEW no existe en un DELETE y OLD no existe en un INSERT: cada rama toca
    -- solo lo que tiene.
    IF TG_OP IN ('INSERT', 'UPDATE') THEN
        UPDATE facultad
           SET total_programas = (SELECT count(*) FROM programa
                                   WHERE facultad = NEW.facultad AND activo)
         WHERE id = NEW.facultad;
    END IF;

    IF TG_OP = 'DELETE'
       OR (TG_OP = 'UPDATE' AND OLD.facultad IS DISTINCT FROM NEW.facultad) THEN
        UPDATE facultad
           SET total_programas = (SELECT count(*) FROM programa
                                   WHERE facultad = OLD.facultad AND activo)
         WHERE id = OLD.facultad;
    END IF;

    -- En un AFTER el valor devuelto se ignora; se devuelve la fila por convención.
    RETURN NULL;
END;
$$;

-- DROP + CREATE porque CREATE TRIGGER no tiene OR REPLACE en todas las versiones.
DROP TRIGGER IF EXISTS trg_contar_programas ON programa;
CREATE TRIGGER trg_contar_programas
    AFTER INSERT OR UPDATE OR DELETE ON programa
    FOR EACH ROW
    EXECUTE FUNCTION fn_contar_programas();
