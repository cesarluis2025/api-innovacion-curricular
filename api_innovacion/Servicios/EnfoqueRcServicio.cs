using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IEnfoqueRcServicio
{
    Task<IEnumerable<EnfoqueRc>> ListarAsync();
    Task<EnfoqueRc> ObtenerPorLlaveAsync(int enfoque, int registroCalificado);
    Task<EnfoqueRc> AsignarAsync(CrearEnfoqueRcPeticion peticion);
    Task RetirarAsync(int enfoque, int registroCalificado);
}

public class EnfoqueRcServicio : IEnfoqueRcServicio
{
    private readonly IEnfoqueRcRepositorio _repositorio;
    public EnfoqueRcServicio(IEnfoqueRcRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<EnfoqueRc>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<EnfoqueRc> ObtenerPorLlaveAsync(int enfoque, int registroCalificado)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(enfoque, registroCalificado);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (enfoque {enfoque}, registro calificado {registroCalificado})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_enfoque_rc reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<EnfoqueRc> AsignarAsync(CrearEnfoqueRcPeticion peticion)
    {
        var fila = new EnfoqueRc
        {
            Enfoque = peticion.Enfoque,
            RegistroCalificado = peticion.RegistroCalificado,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task RetirarAsync(int enfoque, int registroCalificado)
    {
        if (!await _repositorio.RetirarAsync(enfoque, registroCalificado))
            throw new NoEncontradoExcepcion($"no existe la asignación (enfoque {enfoque}, registro calificado {registroCalificado})");
    }
}