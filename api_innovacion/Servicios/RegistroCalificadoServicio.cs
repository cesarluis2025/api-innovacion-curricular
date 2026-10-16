using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IRegistroCalificadoServicio
{
    Task<IEnumerable<RegistroCalificado>> ListarAsync();
    Task<RegistroCalificado> ObtenerPorIdAsync(int codigo);
    Task<RegistroCalificado> CrearAsync(CrearRegistroCalificadoPeticion peticion);
    Task<RegistroCalificado> ActualizarAsync(int codigo, ActualizarRegistroCalificadoPeticion peticion);
    Task EliminarAsync(int codigo);
}

public class RegistroCalificadoServicio : IRegistroCalificadoServicio
{
    private readonly IRegistroCalificadoRepositorio _repositorio;
    public RegistroCalificadoServicio(IRegistroCalificadoRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<RegistroCalificado>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<RegistroCalificado> ObtenerPorIdAsync(int codigo)
    {
        var fila = await _repositorio.ObtenerPorIdAsync(codigo);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe un registro calificado con código {codigo}");
        return fila;
    }

    public async Task<RegistroCalificado> CrearAsync(CrearRegistroCalificadoPeticion peticion)
    {
        // La llave se digita a mano (constitución 1.4): se revisa antes para dar
        // un mensaje claro; si aun así choca (llave retirada), la base responde
        // 23505 y el manejador de errores lo vuelve 409.
        if (await _repositorio.ExisteIdAsync(peticion.Codigo))
            throw new ConflictoExcepcion($"ya existe un registro calificado con código {peticion.Codigo}");

        var fila = new RegistroCalificado
        {
            Codigo = peticion.Codigo,
            CantCreditos = peticion.CantCreditos,
            HoraAcom = peticion.HoraAcom,
            HoraInd = peticion.HoraInd,
            Metodologia = peticion.Metodologia,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            DuracionAnios = peticion.DuracionAnios,
            DuracionSemestres = peticion.DuracionSemestres,
            TipoTitulacion = peticion.TipoTitulacion,
            Programa = peticion.Programa,
        };
        return await _repositorio.CrearAsync(fila);
    }

    public async Task<RegistroCalificado> ActualizarAsync(int codigo, ActualizarRegistroCalificadoPeticion peticion)
    {
        var fila = new RegistroCalificado
        {
            Codigo = codigo,
            CantCreditos = peticion.CantCreditos,
            HoraAcom = peticion.HoraAcom,
            HoraInd = peticion.HoraInd,
            Metodologia = peticion.Metodologia,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            DuracionAnios = peticion.DuracionAnios,
            DuracionSemestres = peticion.DuracionSemestres,
            TipoTitulacion = peticion.TipoTitulacion,
            Programa = peticion.Programa,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe un registro calificado con código {codigo}");
        return guardada;
    }

    public async Task EliminarAsync(int codigo)
    {
        if (!await _repositorio.EliminarLogicoAsync(codigo))
            throw new NoEncontradoExcepcion($"no existe un registro calificado con código {codigo}");
    }
}