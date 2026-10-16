using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IProgramaPeServicio
{
    Task<IEnumerable<ProgramaPe>> ListarAsync();
    Task<ProgramaPe> ObtenerPorLlaveAsync(int programa, int practicaEstrategia);
    Task<ProgramaPe> AsignarAsync(CrearProgramaPePeticion peticion);
    Task RetirarAsync(int programa, int practicaEstrategia);
}

public class ProgramaPeServicio : IProgramaPeServicio
{
    private readonly IProgramaPeRepositorio _repositorio;
    public ProgramaPeServicio(IProgramaPeRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<ProgramaPe>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<ProgramaPe> ObtenerPorLlaveAsync(int programa, int practicaEstrategia)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(programa, practicaEstrategia);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, practica estrategia {practicaEstrategia})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_programa_pe reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<ProgramaPe> AsignarAsync(CrearProgramaPePeticion peticion)
    {
        var fila = new ProgramaPe
        {
            Programa = peticion.Programa,
            PracticaEstrategia = peticion.PracticaEstrategia,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task RetirarAsync(int programa, int practicaEstrategia)
    {
        if (!await _repositorio.RetirarAsync(programa, practicaEstrategia))
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, practica estrategia {practicaEstrategia})");
    }
}