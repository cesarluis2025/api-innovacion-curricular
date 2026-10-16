using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IActivAcademicaServicio
{
    Task<IEnumerable<ActivAcademica>> ListarAsync();
    Task<ActivAcademica> ObtenerPorIdAsync(int id);
    Task<ActivAcademica> CrearAsync(CrearActivAcademicaPeticion peticion);
    Task<ActivAcademica> ActualizarAsync(int id, ActualizarActivAcademicaPeticion peticion);
    Task EliminarAsync(int id);
}

public class ActivAcademicaServicio : IActivAcademicaServicio
{
    private readonly IActivAcademicaRepositorio _repositorio;
    public ActivAcademicaServicio(IActivAcademicaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<ActivAcademica>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<ActivAcademica> ObtenerPorIdAsync(int id)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(id);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe una actividad académica con id {id}");
        return fila;
    }

    public async Task<ActivAcademica> CrearAsync(CrearActivAcademicaPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe una actividad académica con id {peticion.Id}");

        var fila = new ActivAcademica
        {
            Id = peticion.Id,
            Nombre = peticion.Nombre,
            NumCreditos = peticion.NumCreditos,
            Tipo = peticion.Tipo,
            AreaFormacion = peticion.AreaFormacion,
            HAcom = peticion.HAcom,
            HIndep = peticion.HIndep,
            Idioma = peticion.Idioma,
            Espejo = peticion.Espejo,
            EntidadEspejo = TextoEspejo(peticion.Espejo, peticion.EntidadEspejo),
            PaisEspejo = TextoEspejo(peticion.Espejo, peticion.PaisEspejo),
            Disenio = peticion.Disenio,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<ActivAcademica> ActualizarAsync(int id, ActualizarActivAcademicaPeticion peticion)
    {
        var fila = new ActivAcademica
        {
            Id = id,
            Nombre = peticion.Nombre,
            NumCreditos = peticion.NumCreditos,
            Tipo = peticion.Tipo,
            AreaFormacion = peticion.AreaFormacion,
            HAcom = peticion.HAcom,
            HIndep = peticion.HIndep,
            Idioma = peticion.Idioma,
            Espejo = peticion.Espejo,
            EntidadEspejo = TextoEspejo(peticion.Espejo, peticion.EntidadEspejo),
            PaisEspejo = TextoEspejo(peticion.Espejo, peticion.PaisEspejo),
            Disenio = peticion.Disenio,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe una actividad académica con id {id}");
        return guardada;
    }

    public async Task EliminarAsync(int id)
    {
        if (!await _repositorio.EliminarLogicoAsync(id))
            throw new NoEncontradoExcepcion($"no existe una actividad académica con id {id}");
    }

    // D-v2-14: si la actividad no es espejo, la base pide un texto igualmente; se guarda «No aplica».
    private static string TextoEspejo(int espejo, string? texto)
        => espejo == 0 ? "No aplica" : texto!.Trim();
}