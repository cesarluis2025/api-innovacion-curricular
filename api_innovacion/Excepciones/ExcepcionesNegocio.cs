namespace ApiInnovacionCurricular.Excepciones;

// El servicio la lanza cuando piden un registro que no existe (o no está
// activo). El controlador la atrapa y responde 404.
public class NoEncontradoExcepcion : Exception
{
    public NoEncontradoExcepcion(string mensaje) : base(mensaje) { }
}

// El servicio la lanza cuando una regla de negocio choca con lo que ya hay:
// un id repetido, o retirar un maestro que aún tiene detalle activo. Desde
// la v2 el manejador de errores (Middleware/ManejadorErrores.cs) la convierte
// en 409; en la v1 era 400, pero «repetido» es un conflicto, no una petición mal formada.
public class ConflictoExcepcion : Exception
{
    public ConflictoExcepcion(string mensaje) : base(mensaje) { }
}