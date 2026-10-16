using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface ICarInnovacionRepositorio
{
    Task<IEnumerable<CarInnovacion>> ListarAsync();
    Task<CarInnovacion?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(CarInnovacion entidad);
    Task<bool> ActualizarAsync(CarInnovacion entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class CarInnovacionRepositorio : ICarInnovacionRepositorio
{
    private readonly string _cadenaConexion;
    public CarInnovacionRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;

    // Cada método abre su conexión y la cierra sola (el "using").
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    // Desde la v2 el repositorio NO escribe SQL de tablas: solo llama a las
    // funciones sp_ de db/03_sp_v1.sql. Así la regla (filtrar activos, no
    // borrar de verdad) vive en un único lugar, la base. Las columnas llegan
    // en snake_case y Dapper las une a las propiedades gracias a
    // MatchNamesWithUnderscores (Program.cs).

    public async Task<IEnumerable<CarInnovacion>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<CarInnovacion>("SELECT * FROM sp_listar_car_innovacion()");
    }

    public async Task<CarInnovacion?> ObtenerPorIdAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<CarInnovacion>(
            "SELECT * FROM sp_consultar_car_innovacion(@Id)", new { Id = id });
    }

    // Solo ve registros ACTIVOS. Si el id existe pero está retirado, el
    // servicio no lo detecta aquí y el INSERT falla con 23505, que el
    // manejador de errores convierte en 409 (nunca se reutiliza una llave).
    public async Task<bool> ExisteIdAsync(int id)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<CarInnovacion>(
            "SELECT * FROM sp_consultar_car_innovacion(@Id)", new { Id = id });
        return fila is not null;
    }

    // La función devuelve la fila insertada; aquí no se necesita, el servicio
    // ya conoce los valores.
    public async Task CrearAsync(CarInnovacion entidad)
    {
        using var conexion = Conexion;
        await conexion.ExecuteAsync("SELECT * FROM sp_crear_car_innovacion(@Id, @Nombre, @Descripcion, @Tipo)", entidad);
    }

    // La función de actualizar devuelve CERO filas si el id no existe o está
    // retirado; eso es lo que el servicio traduce a 404.
    public async Task<bool> ActualizarAsync(CarInnovacion entidad)
    {
        using var conexion = Conexion;
        var filas = await conexion.QueryAsync<CarInnovacion>("SELECT * FROM sp_actualizar_car_innovacion(@Id, @Nombre, @Descripcion, @Tipo)", entidad);
        return filas.Any();
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_car_innovacion(@Id)", new { Id = id });
    }
}