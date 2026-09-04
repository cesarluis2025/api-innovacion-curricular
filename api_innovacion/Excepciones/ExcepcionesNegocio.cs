namespace ApiInnovacionCurricular.Excepciones;

// El servicio la lanza cuando piden un registro que no existe (o no está
// activo). El controlador la atrapa y responde 404.
public class NoEncontradoExcepcion : Exception
{
    public NoEncontradoExcepcion(string mensaje) : base(mensaje) { }
}

// El servicio la lanza cuando intentan crear un registro con un id que
// ya existe. El controlador la atrapa y responde 400.
public class ConflictoExcepcion : Exception
{
    public ConflictoExcepcion(string mensaje) : base(mensaje) { }
}
