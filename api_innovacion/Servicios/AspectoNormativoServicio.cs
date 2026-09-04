using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAspectoNormativoServicio
{
    Task<IEnumerable<AspectoNormativo>> ListarAsync();
    Task<AspectoNormativo> ObtenerPorIdAsync(int id);
    Task<AspectoNormativo> CrearAsync(CrearAspectoNormativoPeticion peticion);
    Task<AspectoNormativo> ActualizarAsync(int id, ActualizarAspectoNormativoPeticion peticion);
    Task EliminarAsync(int id);
}

public class AspectoNormativoServicio : IAspectoNormativoServicio
{
    private readonly IAspectoNormativoRepositorio _repositorio;
    public AspectoNormativoServicio(IAspectoNormativoRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<AspectoNormativo>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<AspectoNormativo> ObtenerPorIdAsync(int id)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad is null)
            throw new NoEncontradoExcepcion($"no existe un aspecto normativo con id {id}");
        return entidad;
    }

    public async Task<AspectoNormativo> CrearAsync(CrearAspectoNormativoPeticion peticion)
    {
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe un aspecto normativo con id {peticion.Id}");

        var entidad = new AspectoNormativo { Id = peticion.Id, Tipo = peticion.Tipo, Descripcion = peticion.Descripcion, Fuente = peticion.Fuente };
        await _repositorio.CrearAsync(entidad);
        return entidad;
    }

    public async Task<AspectoNormativo> ActualizarAsync(int id, ActualizarAspectoNormativoPeticion peticion)
    {
        var entidad = new AspectoNormativo { Id = id, Tipo = peticion.Tipo, Descripcion = peticion.Descripcion, Fuente = peticion.Fuente };
        if (!await _repositorio.ActualizarAsync(entidad))
            throw new NoEncontradoExcepcion($"no existe un aspecto normativo con id {id}");
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe un aspecto normativo con id {id}");
    }
}
