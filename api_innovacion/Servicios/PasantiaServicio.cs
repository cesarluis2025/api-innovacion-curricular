using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IPasantiaServicio
{
    Task<IEnumerable<Pasantia>> ListarAsync();
    Task<Pasantia> ObtenerPorIdAsync(int id);
    Task<Pasantia> CrearAsync(CrearPasantiaPeticion peticion);
    Task<Pasantia> ActualizarAsync(int id, ActualizarPasantiaPeticion peticion);
    Task EliminarAsync(int id);
}

public class PasantiaServicio : IPasantiaServicio
{
    private readonly IPasantiaRepositorio _repositorio;
    public PasantiaServicio(IPasantiaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Pasantia>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Pasantia> ObtenerPorIdAsync(int id)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(id);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe una pasantía con id {id}");
        return fila;
    }

    public async Task<Pasantia> CrearAsync(CrearPasantiaPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una pasantía con id {peticion.Id}");

        var fila = new Pasantia
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            Pais = peticion.Pais,
            Empresa = peticion.Empresa,
            Descripcion = peticion.Descripcion,
            Programa = peticion.Programa,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<Pasantia> ActualizarAsync(int id, ActualizarPasantiaPeticion peticion)
    {
        var fila = new Pasantia
        {
            Id = id,
            Nombre = peticion.Nombre,
            Pais = peticion.Pais,
            Empresa = peticion.Empresa,
            Descripcion = peticion.Descripcion,
            Programa = peticion.Programa,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe una pasantía con id {id}");
        return guardada;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una pasantía con id {id}");
    }
}