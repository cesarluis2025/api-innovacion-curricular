using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IFacultadServicio
{
    Task<IEnumerable<Facultad>> ListarAsync();
    Task<Facultad> ObtenerPorIdAsync(int id);
    Task<Facultad> CrearAsync(CrearFacultadPeticion peticion);
    Task<Facultad> ActualizarAsync(int id, ActualizarFacultadPeticion peticion);
    Task EliminarAsync(int id);
}

public class FacultadServicio : IFacultadServicio
{
    private readonly IFacultadRepositorio _repositorio;
    public FacultadServicio(IFacultadRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Facultad>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Facultad> ObtenerPorIdAsync(int id)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(id);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe una facultad con id {id}");
        return fila;
    }

    public async Task<Facultad> CrearAsync(CrearFacultadPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una facultad con id {peticion.Id}");

        var fila = new Facultad
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            Tipo = peticion.Tipo,
            FechaFun = peticion.FechaFun,
            Universidad = peticion.Universidad,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<Facultad> ActualizarAsync(int id, ActualizarFacultadPeticion peticion)
    {
        var fila = new Facultad
        {
            Id = id,
            Nombre = peticion.Nombre,
            Tipo = peticion.Tipo,
            FechaFun = peticion.FechaFun,
            Universidad = peticion.Universidad,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe una facultad con id {id}");
        return guardada;
    }

    public async Task EliminarAsync(int id)
    {
        // Regla B1: un maestro con detalle activo no se retira (la pregunta la responde
        // sp_contar_detalle_facultad). Si el id no existe, el conteo es 0 y sigue el 404.
        if (await _repositorio.TieneDetalleActivoAsync(id))
            throw new ConflictoExcepcion($"no se puede retirar la facultad {id}: tiene registros activos");

        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una facultad con id {id}");
    }
}