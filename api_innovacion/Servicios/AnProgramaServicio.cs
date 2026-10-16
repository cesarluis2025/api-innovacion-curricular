using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAnProgramaServicio
{
    Task<IEnumerable<AnPrograma>> ListarAsync();
    Task<AnPrograma> ObtenerPorLlaveAsync(int aspectoNormativo, int programa);
    Task<AnPrograma> AsignarAsync(CrearAnProgramaPeticion peticion);
    Task RetirarAsync(int aspectoNormativo, int programa);
}

public class AnProgramaServicio : IAnProgramaServicio
{
    private readonly IAnProgramaRepositorio _repositorio;
    public AnProgramaServicio(IAnProgramaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<AnPrograma>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<AnPrograma> ObtenerPorLlaveAsync(int aspectoNormativo, int programa)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(aspectoNormativo, programa);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (aspecto normativo {aspectoNormativo}, programa {programa})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_an_programa reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<AnPrograma> AsignarAsync(CrearAnProgramaPeticion peticion)
    {
        var fila = new AnPrograma
        {
            AspectoNormativo = peticion.AspectoNormativo,
            Programa = peticion.Programa,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task RetirarAsync(int aspectoNormativo, int programa)
    {
        if (!await _repositorio.RetirarAsync(aspectoNormativo, programa))
            throw new NoEncontradoExcepcion($"no existe la asignación (aspecto normativo {aspectoNormativo}, programa {programa})");
    }
}