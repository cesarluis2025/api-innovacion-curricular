using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAcreditacionServicio
{
    Task<IEnumerable<Acreditacion>> ListarAsync();
    Task<Acreditacion> ObtenerPorIdAsync(int resolucion);
    Task<Acreditacion> CrearAsync(CrearAcreditacionPeticion peticion);
    Task<Acreditacion> ActualizarAsync(int resolucion, ActualizarAcreditacionPeticion peticion);
    Task EliminarAsync(int resolucion);
}

public class AcreditacionServicio : IAcreditacionServicio
{
    private readonly IAcreditacionRepositorio _repositorio;
    public AcreditacionServicio(IAcreditacionRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Acreditacion>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Acreditacion> ObtenerPorIdAsync(int resolucion)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(resolucion);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe una acreditación con resolución {resolucion}");
        return fila;
    }

    public async Task<Acreditacion> CrearAsync(CrearAcreditacionPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Resolucion))
            throw new ConflictoExcepcion($"ya existe una acreditación con resolución {peticion.Resolucion}");

        var fila = new Acreditacion
        {
            Resolucion = peticion.Resolucion,
            Tipo = peticion.Tipo,
            Calificacion = peticion.Calificacion,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            Programa = peticion.Programa,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<Acreditacion> ActualizarAsync(int resolucion, ActualizarAcreditacionPeticion peticion)
    {
        var fila = new Acreditacion
        {
            Resolucion = resolucion,
            Tipo = peticion.Tipo,
            Calificacion = peticion.Calificacion,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            Programa = peticion.Programa,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe una acreditación con resolución {resolucion}");
        return guardada;
    }

    public async Task EliminarAsync(int resolucion)
    {
        if (!await _repositorio.EliminarLogicoAsync(resolucion))
            throw new NoEncontradoExcepcion($"no existe una acreditación con resolución {resolucion}");
    }
}