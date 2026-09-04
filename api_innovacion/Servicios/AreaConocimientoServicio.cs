using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

public class AreaConocimientoServicio : IAreaConocimientoServicio
{
    private readonly IAreaConocimientoRepositorio _repositorio;

    // No depende del repositorio concreto, sino de su interfaz — así el
    // servicio no sabe (ni le importa) que por debajo hay Dapper y PostgreSQL.
    public AreaConocimientoServicio(IAreaConocimientoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IEnumerable<AreaConocimiento>> ListarAsync()
        => _repositorio.ListarAsync();

    public async Task<AreaConocimiento> ObtenerPorIdAsync(int id)
    {
        var area = await _repositorio.ObtenerPorIdAsync(id);
        if (area is null)
            throw new NoEncontradoExcepcion($"no existe un área de conocimiento con id {id}");
        return area;
    }

    public async Task<AreaConocimiento> CrearAsync(CrearAreaConocimientoPeticion peticion)
    {
        // Regla de negocio del punto 1.4 de la constitución: el id se
        // digita a mano, así que hay que verificar que no esté repetido
        // antes de insertar (la base de datos también lo rechazaría por
        // la llave primaria, pero así devolvemos un mensaje claro).
        if (await _repositorio.ExisteIdAsync(peticion.Id))
            throw new ConflictoExcepcion($"ya existe un área de conocimiento con id {peticion.Id}");

        var area = new AreaConocimiento
        {
            Id = peticion.Id,
            GranArea = peticion.GranArea,
            Area = peticion.Area,
            Disciplina = peticion.Disciplina,
        };
        await _repositorio.CrearAsync(area);
        return area;
    }

    public async Task<AreaConocimiento> ActualizarAsync(int id, ActualizarAreaConocimientoPeticion peticion)
    {
        var area = new AreaConocimiento
        {
            Id = id,
            GranArea = peticion.GranArea,
            Area = peticion.Area,
            Disciplina = peticion.Disciplina,
        };
        var actualizado = await _repositorio.ActualizarAsync(area);
        if (!actualizado)
            throw new NoEncontradoExcepcion($"no existe un área de conocimiento con id {id}");
        return area;
    }

    public async Task EliminarAsync(int id)
    {
        var eliminado = await _repositorio.EliminarLogicoAsync(id);
        if (!eliminado)
            throw new NoEncontradoExcepcion($"no existe un área de conocimiento con id {id}");
    }
}
