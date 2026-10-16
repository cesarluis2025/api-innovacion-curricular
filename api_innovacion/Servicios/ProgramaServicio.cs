using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IProgramaServicio
{
    Task<IEnumerable<Programa>> ListarAsync();
    Task<Programa> ObtenerPorIdAsync(int id);
    Task<Programa> CrearAsync(CrearProgramaPeticion peticion);
    Task<Programa> ActualizarAsync(int id, ActualizarProgramaPeticion peticion);
    Task EliminarAsync(int id);
}

public class ProgramaServicio : IProgramaServicio
{
    private readonly IProgramaRepositorio _repositorio;
    public ProgramaServicio(IProgramaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Programa>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Programa> ObtenerPorIdAsync(int id)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(id);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe un programa con id {id}");
        return fila;
    }

    public async Task<Programa> CrearAsync(CrearProgramaPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe un programa con id {peticion.Id}");

        var fila = new Programa
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            Tipo = peticion.Tipo,
            Nivel = peticion.Nivel,
            FechaCreacion = peticion.FechaCreacion,
            FechaCierre = peticion.FechaCierre,
            NumeroCohortes = peticion.NumeroCohortes,
            CantGraduados = peticion.CantGraduados,
            FechaActualizacion = peticion.FechaActualizacion,
            Ciudad = peticion.Ciudad,
            Facultad = peticion.Facultad,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<Programa> ActualizarAsync(int id, ActualizarProgramaPeticion peticion)
    {
        var fila = new Programa
        {
            Id = id,
            Nombre = peticion.Nombre,
            Tipo = peticion.Tipo,
            Nivel = peticion.Nivel,
            FechaCreacion = peticion.FechaCreacion,
            FechaCierre = peticion.FechaCierre,
            NumeroCohortes = peticion.NumeroCohortes,
            CantGraduados = peticion.CantGraduados,
            FechaActualizacion = peticion.FechaActualizacion,
            Ciudad = peticion.Ciudad,
            Facultad = peticion.Facultad,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe un programa con id {id}");
        return guardada;
    }

    public async Task EliminarAsync(int id)
    {
        // Regla B1: un maestro con detalle activo no se retira (la pregunta la responde
        // sp_contar_detalle_programa). Si el id no existe, el conteo es 0 y sigue el 404.
        if (await _repositorio.TieneDetalleActivoAsync(id))
            throw new ConflictoExcepcion($"no se puede retirar el programa {id}: tiene registros activos");

        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe un programa con id {id}");
    }
}