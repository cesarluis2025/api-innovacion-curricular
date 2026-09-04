using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IEnfoqueServicio
{
    Task<IEnumerable<Enfoque>> ListarAsync();
    Task<Enfoque> ObtenerPorIdAsync(int id);
    Task<Enfoque> CrearAsync(CrearEnfoquePeticion peticion);
    Task<Enfoque> ActualizarAsync(int id, ActualizarEnfoquePeticion peticion);
    Task EliminarAsync(int id);
}

public class EnfoqueServicio : IEnfoqueServicio
{
    private readonly IEnfoqueRepositorio _repositorio;
    public EnfoqueServicio(IEnfoqueRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Enfoque>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Enfoque> ObtenerPorIdAsync(int id)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad is null)
            throw new NoEncontradoExcepcion($"no existe un enfoque con id {id}");
        return entidad;
    }

    public async Task<Enfoque> CrearAsync(CrearEnfoquePeticion peticion)
    {
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe un enfoque con id {peticion.Id}");

        var entidad = new Enfoque { Id = peticion.Id, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion };
        await _repositorio.CrearAsync(entidad);
        return entidad;
    }

    public async Task<Enfoque> ActualizarAsync(int id, ActualizarEnfoquePeticion peticion)
    {
        var entidad = new Enfoque { Id = id, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion };
        if (!await _repositorio.ActualizarAsync(entidad))
            throw new NoEncontradoExcepcion($"no existe un enfoque con id {id}");
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe un enfoque con id {id}");
    }
}
