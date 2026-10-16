using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IProgramaCiServicio
{
    Task<IEnumerable<ProgramaCi>> ListarAsync();
    Task<ProgramaCi> ObtenerPorLlaveAsync(int programa, int carInnovacion);
    Task<ProgramaCi> AsignarAsync(CrearProgramaCiPeticion peticion);
    Task RetirarAsync(int programa, int carInnovacion);
}

public class ProgramaCiServicio : IProgramaCiServicio
{
    private readonly IProgramaCiRepositorio _repositorio;
    public ProgramaCiServicio(IProgramaCiRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<ProgramaCi>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<ProgramaCi> ObtenerPorLlaveAsync(int programa, int carInnovacion)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(programa, carInnovacion);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, car innovacion {carInnovacion})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_programa_ci reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<ProgramaCi> AsignarAsync(CrearProgramaCiPeticion peticion)
    {
        var fila = new ProgramaCi
        {
            Programa = peticion.Programa,
            CarInnovacion = peticion.CarInnovacion,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task RetirarAsync(int programa, int carInnovacion)
    {
        if (!await _repositorio.RetirarAsync(programa, carInnovacion))
            throw new NoEncontradoExcepcion($"no existe la asignación (programa {programa}, car innovacion {carInnovacion})");
    }
}