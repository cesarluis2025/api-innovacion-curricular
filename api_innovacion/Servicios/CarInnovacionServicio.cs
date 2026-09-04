using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface ICarInnovacionServicio
{
    Task<IEnumerable<CarInnovacion>> ListarAsync();
    Task<CarInnovacion> ObtenerPorIdAsync(int id);
    Task<CarInnovacion> CrearAsync(CrearCarInnovacionPeticion peticion);
    Task<CarInnovacion> ActualizarAsync(int id, ActualizarCarInnovacionPeticion peticion);
    Task EliminarAsync(int id);
}

public class CarInnovacionServicio : ICarInnovacionServicio
{
    private readonly ICarInnovacionRepositorio _repositorio;
    public CarInnovacionServicio(ICarInnovacionRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<CarInnovacion>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<CarInnovacion> ObtenerPorIdAsync(int id)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad is null)
            throw new NoEncontradoExcepcion($"no existe una característica de innovación con id {id}");
        return entidad;
    }

    public async Task<CarInnovacion> CrearAsync(CrearCarInnovacionPeticion peticion)
    {
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una característica de innovación con id {peticion.Id}");

        var entidad = new CarInnovacion { Id = peticion.Id, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion, Tipo = peticion.Tipo };
        await _repositorio.CrearAsync(entidad);
        return entidad;
    }

    public async Task<CarInnovacion> ActualizarAsync(int id, ActualizarCarInnovacionPeticion peticion)
    {
        var entidad = new CarInnovacion { Id = id, Nombre = peticion.Nombre, Descripcion = peticion.Descripcion, Tipo = peticion.Tipo };
        if (!await _repositorio.ActualizarAsync(entidad))
            throw new NoEncontradoExcepcion($"no existe una característica de innovación con id {id}");
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una característica de innovación con id {id}");
    }
}
