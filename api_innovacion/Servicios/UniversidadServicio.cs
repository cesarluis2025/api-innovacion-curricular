using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IUniversidadServicio
{
    Task<IEnumerable<Universidad>> ListarAsync();
    Task<Universidad> ObtenerPorIdAsync(int id);
    Task<Universidad> CrearAsync(CrearUniversidadPeticion peticion);
    Task<Universidad> ActualizarAsync(int id, ActualizarUniversidadPeticion peticion);
    Task EliminarAsync(int id);
}

public class UniversidadServicio : IUniversidadServicio
{
    private readonly IUniversidadRepositorio _repositorio;
    public UniversidadServicio(IUniversidadRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Universidad>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Universidad> ObtenerPorIdAsync(int id)
    {
        var universidad = await _repositorio.ObtenerPorIdAsync(id);
        if (universidad is null)
            throw new NoEncontradoExcepcion($"no existe una universidad con id {id}");
        return universidad;
    }

    public async Task<Universidad> CrearAsync(CrearUniversidadPeticion peticion)
    {
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una universidad con id {peticion.Id}");

        var universidad = new Universidad { Id = peticion.Id, Nombre = peticion.Nombre, Tipo = peticion.Tipo, Ciudad = peticion.Ciudad };
        await _repositorio.CrearAsync(universidad);
        return universidad;
    }

    public async Task<Universidad> ActualizarAsync(int id, ActualizarUniversidadPeticion peticion)
    {
        var universidad = new Universidad { Id = id, Nombre = peticion.Nombre, Tipo = peticion.Tipo, Ciudad = peticion.Ciudad };
        if (!await _repositorio.ActualizarAsync(universidad))
            throw new NoEncontradoExcepcion($"no existe una universidad con id {id}");
        return universidad;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una universidad con id {id}");
    }
}
