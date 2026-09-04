using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

// El servicio depende de esta interfaz, no de la clase concreta. Así, si
// mañana cambian de motor de base de datos, solo se reescribe la
// implementación, no el resto de la aplicación.
public interface IAreaConocimientoRepositorio
{
    Task<IEnumerable<AreaConocimiento>> ListarAsync();
    Task<AreaConocimiento?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(AreaConocimiento area);
    Task<bool> ActualizarAsync(AreaConocimiento area);
    Task<bool> EliminarLogicoAsync(int id);
}
