using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAaRcServicio
{
    Task<IEnumerable<AaRc>> ListarAsync();
    Task<AaRc> ObtenerPorLlaveAsync(int activAcademicasIdcurso, int registroCalificadoCodigo);
    Task<AaRc> AsignarAsync(CrearAaRcPeticion peticion);
    Task<AaRc> ActualizarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo, ActualizarAaRcPeticion peticion);
    Task RetirarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo);
}

public class AaRcServicio : IAaRcServicio
{
    private readonly IAaRcRepositorio _repositorio;
    public AaRcServicio(IAaRcRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<AaRc>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<AaRc> ObtenerPorLlaveAsync(int activAcademicasIdcurso, int registroCalificadoCodigo)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(activAcademicasIdcurso, registroCalificadoCodigo);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (activ academicas idcurso {activAcademicasIdcurso}, registro calificado codigo {registroCalificadoCodigo})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_aa_rc reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<AaRc> AsignarAsync(CrearAaRcPeticion peticion)
    {
        var fila = new AaRc
        {
            ActivAcademicasIdcurso = peticion.ActivAcademicasIdcurso,
            RegistroCalificadoCodigo = peticion.RegistroCalificadoCodigo,
            Componente = peticion.Componente,
            Semestre = peticion.Semestre,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task<AaRc> ActualizarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo, ActualizarAaRcPeticion peticion)
    {
        var fila = new AaRc
        {
            ActivAcademicasIdcurso = activAcademicasIdcurso,
            RegistroCalificadoCodigo = registroCalificadoCodigo,
            Componente = peticion.Componente,
            Semestre = peticion.Semestre,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (activ academicas idcurso {activAcademicasIdcurso}, registro calificado codigo {registroCalificadoCodigo})");
        return guardada;
    }

    public async Task RetirarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo)
    {
        if (!await _repositorio.RetirarAsync(activAcademicasIdcurso, registroCalificadoCodigo))
            throw new NoEncontradoExcepcion($"no existe la asignación (activ academicas idcurso {activAcademicasIdcurso}, registro calificado codigo {registroCalificadoCodigo})");
    }
}