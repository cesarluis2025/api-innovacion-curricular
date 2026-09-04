using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IPracticaEstrategiaServicio
{
    Task<IEnumerable<PracticaEstrategia>> ListarAsync();
    Task<PracticaEstrategia> ObtenerPorIdAsync(int id);
    Task<PracticaEstrategia> CrearAsync(CrearPracticaEstrategiaPeticion peticion);
    Task<PracticaEstrategia> ActualizarAsync(int id, ActualizarPracticaEstrategiaPeticion peticion);
    Task EliminarAsync(int id);
}

public class PracticaEstrategiaServicio : IPracticaEstrategiaServicio
{
    private readonly IPracticaEstrategiaRepositorio _repositorio;
    public PracticaEstrategiaServicio(IPracticaEstrategiaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<PracticaEstrategia>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<PracticaEstrategia> ObtenerPorIdAsync(int id)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad is null)
            throw new NoEncontradoExcepcion($"no existe una práctica/estrategia con id {id}");
        return entidad;
    }

    public async Task<PracticaEstrategia> CrearAsync(CrearPracticaEstrategiaPeticion peticion)
    {
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una práctica/estrategia con id {peticion.Id}");

        var entidad = new PracticaEstrategia { Id = peticion.Id, Tipo = peticion.Tipo, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion };
        await _repositorio.CrearAsync(entidad);
        return entidad;
    }

    public async Task<PracticaEstrategia> ActualizarAsync(int id, ActualizarPracticaEstrategiaPeticion peticion)
    {
        var entidad = new PracticaEstrategia { Id = id, Tipo = peticion.Tipo, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion };
        if (!await _repositorio.ActualizarAsync(entidad))
            throw new NoEncontradoExcepcion($"no existe una práctica/estrategia con id {id}");
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una práctica/estrategia con id {id}");
    }
}
