using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

// Maestro-detalle de Universidad: el detalle no existe sin su maestro, así que se lee y se
// agrega DESDE el maestro (6_contracts.md). Primero se comprueba que el maestro exista,
// para responder 404 antes de tocar el detalle.
public interface IUniversidadDetalleServicio
{
    Task<IEnumerable<Facultad>> ListarFacultadesAsync(int id);
    Task<IEnumerable<Facultad>> CrearFacultadesLoteAsync(int id, LoteFacultadPeticion peticion);
}

public class UniversidadDetalleServicio : IUniversidadDetalleServicio
{
    private readonly IUniversidadRepositorio _maestros;
    private readonly IFacultadRepositorio _facultad;

    public UniversidadDetalleServicio(IUniversidadRepositorio maestros, IFacultadRepositorio facultad)
    {
        _maestros = maestros;
        _facultad = facultad;
    }

    private async Task ExigirMaestroAsync(int id)
    {
        if (await _maestros.ObtenerPorIdAsync(id) is null)
            throw new NoEncontradoExcepcion($"no existe una universidad con id {id}");
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<Facultad>> ListarFacultadesAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _facultad.ListarAsync()).Where(x => x.Universidad == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_facultad_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<Facultad>> CrearFacultadesLoteAsync(int id, LoteFacultadPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new Facultad
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Tipo = r.Tipo,
            FechaFun = r.FechaFun,
        }).ToList();
        return await _facultad.CrearLoteAsync(id, filas);
    }
}