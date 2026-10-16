using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IProgramaAcServicio
{
    Task<IEnumerable<ProgramaAc>> ListarAsync();
    Task<ProgramaAc> ObtenerPorLlaveAsync(int programa, int areaConocimiento);
    Task<ProgramaAc> AsignarAsync(CrearProgramaAcPeticion peticion);
    Task RetirarAsync(int programa, int areaConocimiento);
}

public class ProgramaAcServicio : IProgramaAcServicio
{
    private readonly IProgramaAcRepositorio _repositorio;
    public ProgramaAcServicio(IProgramaAcRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<ProgramaAc>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<ProgramaAc> ObtenerPorLlaveAsync(int programa, int areaConocimiento)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(programa, areaConocimiento);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, area conocimiento {areaConocimiento})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_programa_ac reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<ProgramaAc> AsignarAsync(CrearProgramaAcPeticion peticion)
    {
        var fila = new ProgramaAc
        {
            Programa = peticion.Programa,
            AreaConocimiento = peticion.AreaConocimiento,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task RetirarAsync(int programa, int areaConocimiento)
    {
        if (!await _repositorio.RetirarAsync(programa, areaConocimiento))
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, area conocimiento {areaConocimiento})");
    }
}