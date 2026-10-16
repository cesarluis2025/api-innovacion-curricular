using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IFacultadRepositorio
{
    Task<IEnumerable<Facultad>> ListarAsync();
    Task<Facultad?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task<Facultad> CrearAsync(Facultad entidad);
    Task<Facultad?> ActualizarAsync(Facultad entidad);
    Task<bool> EliminarLogicoAsync(int id);
    Task<IEnumerable<Facultad>> CrearLoteAsync(int maestro, IEnumerable<Facultad> renglones);
    Task<bool> TieneDetalleActivoAsync(int id);
}

// Solo llama a las funciones sp_ de db/04_sp_v2.sql: la API no escribe SQL de
// tablas. Las fechas llevan ::date porque Npgsql envía un DateTime como
// timestamp, y la función espera date (no hay conversión implícita).
public class FacultadRepositorio : IFacultadRepositorio
{
    private readonly string _cadenaConexion;
    public FacultadRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Facultad>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<Facultad>("SELECT * FROM sp_listar_facultad()");
    }

    public async Task<Facultad?> ObtenerPorIdAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Facultad>(
            "SELECT * FROM sp_consultar_facultad(@Id)", new { Id = id });
    }

    // Solo ve registros ACTIVOS: si la llave existe pero está retirada, el
    // INSERT falla con 23505 y el manejador de errores responde 409.
    public async Task<bool> ExisteIdAsync(int id)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<Facultad>(
            "SELECT * FROM sp_consultar_facultad(@Id)", new { Id = id });
        return fila is not null;
    }

    // Devuelve la fila como quedó guardada (valores por defecto incluidos),
    // que es lo que el contrato pide en el 201.
    public async Task<Facultad> CrearAsync(Facultad entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<Facultad>("SELECT * FROM sp_crear_facultad(@Id, @Nombre, @Tipo, @FechaFun::date, @Universidad)", entidad);
    }

    // La función devuelve CERO filas si la llave no existe o está retirada:
    // aquí eso es null y el servicio lo traduce a 404.
    public async Task<Facultad?> ActualizarAsync(Facultad entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Facultad>("SELECT * FROM sp_actualizar_facultad(@Id, @Nombre, @Tipo, @FechaFun::date, @Universidad)", entidad);
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_facultad(@Id)", new { Id = id });
    }

    // Varios renglones en UNA llamada = UNA transacción (D-v2-5): la función
    // guarda todos o ninguno. El JSON usa los nombres de las COLUMNAS (snake_case)
    // porque así los lee jsonb_to_recordset dentro de la función; se arma a mano
    // y no con una política de nombres para que no dependa de cómo se parta «HAcom».
    public async Task<IEnumerable<Facultad>> CrearLoteAsync(int maestro, IEnumerable<Facultad> renglones)
    {
        var json = JsonSerializer.Serialize(renglones.Select(r => new Dictionary<string, object?>
        {
            ["id"] = r.Id,
            ["nombre"] = r.Nombre,
            ["tipo"] = r.Tipo,
            ["fecha_fun"] = r.FechaFun.ToString("yyyy-MM-dd")
        }));
        using var conexion = Conexion;
        return await conexion.QueryAsync<Facultad>(
            "SELECT * FROM sp_crear_facultad_lote(@Maestro, @Renglones::jsonb)", new { Maestro = maestro, Renglones = json });
    }

    // Pregunta a sp_contar_detalle_facultad: la regla de retiro (B1) vive en el servicio y
    // aquí solo está la consulta (D-v2-4).
    public async Task<bool> TieneDetalleActivoAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(
            "SELECT * FROM sp_contar_detalle_facultad(@Id)", new { Id = id }) > 0;
    }
}