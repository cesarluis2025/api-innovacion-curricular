using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

// Maestro-detalle de Facultad: el detalle no existe sin su maestro, así que se lee y se
// agrega DESDE el maestro (6_contracts.md). Primero se comprueba que el maestro exista,
// para responder 404 antes de tocar el detalle.
public interface IFacultadDetalleServicio
{
    Task<IEnumerable<Programa>> ListarProgramasAsync(int id);
    Task<IEnumerable<Programa>> CrearProgramasLoteAsync(int id, LoteProgramaPeticion peticion);
}

public class FacultadDetalleServicio : IFacultadDetalleServicio
{
    private readonly IFacultadRepositorio _maestros;
    private readonly IProgramaRepositorio _programa;

    public FacultadDetalleServicio(IFacultadRepositorio maestros, IProgramaRepositorio programa)
    {
        _maestros = maestros;
        _programa = programa;
    }

    private async Task ExigirMaestroAsync(int id)
    {
        if (await _maestros.ObtenerPorIdAsync(id) is null)
            throw new NoEncontradoExcepcion($"no existe una facultad con id {id}");
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<Programa>> ListarProgramasAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _programa.ListarAsync()).Where(x => x.Facultad == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_programa_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<Programa>> CrearProgramasLoteAsync(int id, LoteProgramaPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new Programa
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Tipo = r.Tipo,
            Nivel = r.Nivel,
            FechaCreacion = r.FechaCreacion,
            FechaCierre = r.FechaCierre,
            NumeroCohortes = r.NumeroCohortes,
            CantGraduados = r.CantGraduados,
            FechaActualizacion = r.FechaActualizacion,
            Ciudad = r.Ciudad,
        }).ToList();
        return await _programa.CrearLoteAsync(id, filas);
    }
}