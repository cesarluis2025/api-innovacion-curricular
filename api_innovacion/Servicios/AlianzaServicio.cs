using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAlianzaServicio
{
    Task<IEnumerable<Alianza>> ListarAsync();
    Task<Alianza> ObtenerPorLlaveAsync(int aliado, int departamento);
    Task<Alianza> AsignarAsync(CrearAlianzaPeticion peticion);
    Task<Alianza> ActualizarAsync(int aliado, int departamento, ActualizarAlianzaPeticion peticion);
    Task RetirarAsync(int aliado, int departamento);
}

public class AlianzaServicio : IAlianzaServicio
{
    private readonly IAlianzaRepositorio _repositorio;
    public AlianzaServicio(IAlianzaRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Alianza>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Alianza> ObtenerPorLlaveAsync(int aliado, int departamento)
    {
        var fila = await _repositorio.ObtenerPorLlaveAsync(aliado, departamento);
        if (fila is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (aliado {aliado}, departamento {departamento})");
        return fila;
    }

    // No hay revisión previa de «ya existe»: la función sp_crear_alianza reactiva la
    // pareja retirada y la base rechaza la activa repetida con 23505 (409).
    public Task<Alianza> AsignarAsync(CrearAlianzaPeticion peticion)
    {
        var fila = new Alianza
        {
            Aliado = peticion.Aliado,
            Departamento = peticion.Departamento,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            Docente = peticion.Docente,
        };
        return _repositorio.AsignarAsync(fila);
    }

    public async Task<Alianza> ActualizarAsync(int aliado, int departamento, ActualizarAlianzaPeticion peticion)
    {
        var fila = new Alianza
        {
            Aliado = aliado,
            Departamento = departamento,
            FechaInicio = peticion.FechaInicio,
            FechaFin = peticion.FechaFin,
            Docente = peticion.Docente,
        };
        var guardada = await _repositorio.ActualizarAsync(fila);
        if (guardada is null)
            throw new NoEncontradoExcepcion($"no existe la asignación (aliado {aliado}, departamento {departamento})");
        return guardada;
    }

    public async Task RetirarAsync(int aliado, int departamento)
    {
        if (!await _repositorio.RetirarAsync(aliado, departamento))
            throw new NoEncontradoExcepcion($"no existe la asignación (aliado {aliado}, departamento {departamento})");
    }
}