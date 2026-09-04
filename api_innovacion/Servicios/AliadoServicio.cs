using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public interface IAliadoServicio
{
    Task<IEnumerable<Aliado>> ListarAsync();
    Task<Aliado> ObtenerPorNitAsync(int nit);
    Task<Aliado> CrearAsync(CrearAliadoPeticion peticion);
    Task<Aliado> ActualizarAsync(int nit, ActualizarAliadoPeticion peticion);
    Task EliminarAsync(int nit);
}

public class AliadoServicio : IAliadoServicio
{
    private readonly IAliadoRepositorio _repositorio;
    public AliadoServicio(IAliadoRepositorio repositorio) => _repositorio = repositorio;

    public Task<IEnumerable<Aliado>> ListarAsync() => _repositorio.ListarAsync();

    public async Task<Aliado> ObtenerPorNitAsync(int nit)
    {
        var aliado = await _repositorio.ObtenerPorNitAsync(nit);
        if (aliado is null)
            throw new NoEncontradoExcepcion($"no existe un aliado con nit {nit}");
        return aliado;
    }

    public async Task<Aliado> CrearAsync(CrearAliadoPeticion peticion)
    {
        if (await _repositorio.ExisteNitAsync(peticion.Nit))
            throw new ConflictoExcepcion($"ya existe un aliado con nit {peticion.Nit}");

        var aliado = new Aliado
        {
            Nit = peticion.Nit,
            RazonSocial = peticion.RazonSocial,
            NombreContacto = peticion.NombreContacto,
            Correo = peticion.Correo,
            Telefono = peticion.Telefono,
            Ciudad = peticion.Ciudad,
        };
        await _repositorio.CrearAsync(aliado);
        return aliado;
    }

    public async Task<Aliado> ActualizarAsync(int nit, ActualizarAliadoPeticion peticion)
    {
        var aliado = new Aliado
        {
            Nit = nit,
            RazonSocial = peticion.RazonSocial,
            NombreContacto = peticion.NombreContacto,
            Correo = peticion.Correo,
            Telefono = peticion.Telefono,
            Ciudad = peticion.Ciudad,
        };
        if (!await _repositorio.ActualizarAsync(aliado))
            throw new NoEncontradoExcepcion($"no existe un aliado con nit {nit}");
        return aliado;
    }

    public async Task EliminarAsync(int nit)
    {
        if (!await _repositorio.EliminarLogicoAsync(nit))
            throw new NoEncontradoExcepcion($"no existe un aliado con nit {nit}");
    }
}
