using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IPremioRepositorio
{
    Task<IEnumerable<Premio>> ListarAsync();
    Task<Premio?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task<Premio> CrearAsync(Premio entidad);
    Task<Premio?> ActualizarAsync(Premio entidad);
    Task<bool> EliminarLogicoAsync(int id);
    Task<IEnumerable<Premio>> CrearLoteAsync(int maestro, IEnumerable<Premio> renglones);
}

// Solo llama a las funciones sp_ de db/04_sp_v2.sql: la API no escribe SQL de
// tablas. Las fechas llevan ::date porque Npgsql envía un DateTime como
// timestamp, y la función espera date (no hay conversión implícita).
public class PremioRepositorio : IPremioRepositorio
{
    private readonly string _cadenaConexion;
    public PremioRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Premio>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<Premio>("SELECT * FROM sp_listar_premio()");
    }

    public async Task<Premio?> ObtenerPorIdAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Premio>(
            "SELECT * FROM sp_consultar_premio(@Id)", new { Id = id });
    }

    // Solo ve registros ACTIVOS: si la llave existe pero está retirada, el
    // INSERT falla con 23505 y el manejador de errores responde 409.
    public async Task<bool> ExisteIdAsync(int id)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<Premio>(
            "SELECT * FROM sp_consultar_premio(@Id)", new { Id = id });
        return fila is not null;
    }

    // Devuelve la fila como quedó guardada (valores por defecto incluidos),
    // que es lo que el contrato pide en el 201.
    public async Task<Premio> CrearAsync(Premio entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<Premio>("SELECT * FROM sp_crear_premio(@Id, @Nombre, @Descripcion, @Fecha::date, @EntidadOtorgante, @Pais, @Programa)", entidad);
    }

    // La función devuelve CERO filas si la llave no existe o está retirada:
    // aquí eso es null y el servicio lo traduce a 404.
    public async Task<Premio?> ActualizarAsync(Premio entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Premio>("SELECT * FROM sp_actualizar_premio(@Id, @Nombre, @Descripcion, @Fecha::date, @EntidadOtorgante, @Pais, @Programa)", entidad);
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_premio(@Id)", new { Id = id });
    }

    // Varios renglones en UNA llamada = UNA transacción (D-v2-5): la función
    // guarda todos o ninguno. El JSON usa los nombres de las COLUMNAS (snake_case)
    // porque así los lee jsonb_to_recordset dentro de la función; se arma a mano
    // y no con una política de nombres para que no dependa de cómo se parta «HAcom».
    public async Task<IEnumerable<Premio>> CrearLoteAsync(int maestro, IEnumerable<Premio> renglones)
    {
        var json = JsonSerializer.Serialize(renglones.Select(r => new Dictionary<string, object?>
        {
            ["id"] = r.Id,
            ["nombre"] = r.Nombre,
            ["descripcion"] = r.Descripcion,
            ["fecha"] = r.Fecha.ToString("yyyy-MM-dd"),
            ["entidad_otorgante"] = r.EntidadOtorgante,
            ["pais"] = r.Pais
        }));
        using var conexion = Conexion;
        return await conexion.QueryAsync<Premio>(
            "SELECT * FROM sp_crear_premio_lote(@Maestro, @Renglones::jsonb)", new { Maestro = maestro, Renglones = json });
    }
}