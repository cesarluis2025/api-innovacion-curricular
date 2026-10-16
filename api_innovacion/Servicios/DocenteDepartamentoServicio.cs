using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IDocenteDepartamentoServicio
{
    Task<IEnumerable<DocenteDepartamento>> ListarAsync();
    Task<DocenteDepartamento> ObtenerPorLlaveAsync(int docente, int departamento);
    Task<DocenteDepartamento> AsignarAsync(CrearDocenteDepartamentoPeticion peticion);
    Task<DocenteDepartamento> ActualizarAsync(int docente, int departamento, ActualizarDocenteDepartamentoPeticion peticion);
    Task RetirarAsync(int docente, int departamento);
}

public class DocenteDepartamentoServicio : IDocenteDepartamentoServicio
{
    private readonly IDocenteDepartamentoRepositorio _repositorio;
    public DocenteDepartamentoServicio(IDocenteDepartamentoRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<DocenteDepartamento>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<DocenteDepartamento> ObtenerPorLlaveAsync(int docente, int departamento)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(docente, departamento);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (docente {docente}, departamento {departamento})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_docente_departamento reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<DocenteDepartamento> AsignarAsync(CrearDocenteDepartamentoPeticion peticion)
    {
        var fila = new DocenteDepartamento
        {
            Docente = peticion.Docente,
            Departamento = peticion.Departamento,
            Dedicacion = peticion.Dedicacion,
            Modalidad = peticion.Modalidad,
            FechaIngreso = peticion.FechaIngreso,
            FechaSalida = peticion.FechaSalida,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task<DocenteDepartamento> ActualizarAsync(int docente, int departamento, ActualizarDocenteDepartamentoPeticion peticion)
    {
        var fila = new DocenteDepartamento
        {
            Docente = docente,
            Departamento = departamento,
            Dedicacion = peticion.Dedicacion,
            Modalidad = peticion.Modalidad,
            FechaIngreso = peticion.FechaIngreso,
            FechaSalida = peticion.FechaSalida,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (docente {docente}, departamento {departamento})");
        return guardada;
    }

    public async Task RetirarAsync(int docente, int departamento)
    {
        if (!await _repositorio.RetirarAsync(docente, departamento))
            throw new NoEncontradoExcepcion($"no existe la asignación (docente {docente}, departamento {departamento})");
    }
}