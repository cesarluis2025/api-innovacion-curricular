using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IPremioServicio
{
    Task<IEnumerable<Premio>> ListarAsync();
    Task<Premio> ObtenerPorIdAsync(int id);
    Task<Premio> CrearAsync(CrearPremioPeticion peticion);
    Task<Premio> ActualizarAsync(int id, ActualizarPremioPeticion peticion);
    Task EliminarAsync(int id);
}

public class PremioServicio : IPremioServicio
{
    private readonly IPremioRepositorio _repositorio;
    public PremioServicio(IPremioRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Premio>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Premio> ObtenerPorIdAsync(int id)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(id);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe un premio con id {id}");
        return fila;
    }

    public async Task<Premio> CrearAsync(CrearPremioPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe un premio con id {peticion.Id}");

        var fila = new Premio
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            Descripcion = peticion.Descripcion,
            Fecha = peticion.Fecha,
            EntidadOtorgante = peticion.EntidadOtorgante,
            Pais = peticion.Pais,
            Programa = peticion.Programa,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<Premio> ActualizarAsync(int id, ActualizarPremioPeticion peticion)
    {
        var fila = new Premio
        {
            Id = id,
            Nombre = peticion.Nombre,
            Descripcion = peticion.Descripcion,
            Fecha = peticion.Fecha,
            EntidadOtorgante = peticion.EntidadOtorgante,
            Pais = peticion.Pais,
            Programa = peticion.Programa,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe un premio con id {id}");
        return guardada;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe un premio con id {id}");
    }
}