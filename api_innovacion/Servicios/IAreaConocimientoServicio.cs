using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;

namespace ApiInnovacionCurricular.Servicios;

public interface IAreaConocimientoServicio
{
    Task<IEnumerable<AreaConocimiento>> ListarAsync();
    Task<AreaConocimiento> ObtenerPorIdAsync(int id);
    Task<AreaConocimiento> CrearAsync(CrearAreaConocimientoPeticion peticion);
    Task<AreaConocimiento> ActualizarAsync(int id, ActualizarAreaConocimientoPeticion peticion);
    Task EliminarAsync(int id);
}
