using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAcreditacionRepositorio
{
    Task<IEnumerable<Acreditacion>> ListarAsync();
    Task<Acreditacion?> ObtenerPorIdAsync(int resolucion);
    Task<bool> ExisteIdAsync(int resolucion);
    Task<Acreditacion> CrearAsync(Acreditacion entidad);
    Task<Acreditacion?> ActualizarAsync(Acreditacion entidad);
    Task<bool> EliminarLogicoAsync(int resolucion);
    Task<IEnumerable<Acreditacion>> CrearLoteAsync(int maestro, IEnumerable<Acreditacion> renglones);
}

// Solo llama a las funciones sp_ de db/04_sp_v2.sql: la API no escribe SQL de
// tablas. Las fechas llevan ::date porque Npgsql envía un DateTime como
// timestamp, y la función espera date (no hay conversión implícita).
public class AcreditacionRepositorio : IAcreditacionRepositorio
{
    private readonly string _cadenaConexion;
    public AcreditacionRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Acreditacion>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<Acreditacion>("SELECT * FROM sp_listar_acreditacion()");
    }

    public async Task<Acreditacion?> ObtenerPorIdAsync(int resolucion)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Acreditacion>(
            "SELECT * FROM sp_consultar_acreditacion(@Resolucion)", new { Resolucion = resolucion });
    }

    // Solo ve registros ACTIVOS: si la llave existe pero está retirada, el
    // INSERT falla con 23505 y el manejador de errores responde 409.
    public async Task<bool> ExisteIdAsync(int resolucion)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<Acreditacion>(
            "SELECT * FROM sp_consultar_acreditacion(@Resolucion)", new { Resolucion = resolucion });
        return fila is not null;
    }

    // Devuelve la fila como quedó guardada (valores por defecto incluidos),
    // que es lo que el contrato pide en el 201.
    public async Task<Acreditacion> CrearAsync(Acreditacion entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<Acreditacion>("SELECT * FROM sp_crear_acreditacion(@Resolucion, @Tipo, @Calificacion, @FechaInicio, @FechaFin, @Programa)", entidad);
    }

    // La función devuelve CERO filas si la llave no existe o está retirada:
    // aquí eso es null y el servicio lo traduce a 404.
    public async Task<Acreditacion?> ActualizarAsync(Acreditacion entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Acreditacion>("SELECT * FROM sp_actualizar_acreditacion(@Resolucion, @Tipo, @Calificacion, @FechaInicio, @FechaFin, @Programa)", entidad);
    }

    public async Task<bool> EliminarLogicoAsync(int resolucion)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_acreditacion(@Resolucion)", new { Resolucion = resolucion });
    }

    // Varios renglones en UNA llamada = UNA transacción (D-v2-5): la función
    // guarda todos o ninguno. El JSON usa los nombres de las COLUMNAS (snake_case)
    // porque así los lee jsonb_to_recordset dentro de la función; se arma a mano
    // y no con una política de nombres para que no dependa de cómo se parta «HAcom».
    public async Task<IEnumerable<Acreditacion>> CrearLoteAsync(int maestro, IEnumerable<Acreditacion> renglones)
    {
        var json = JsonSerializer.Serialize(renglones.Select(r => new Dictionary<string, object?>
        {
            ["resolucion"] = r.Resolucion,
            ["tipo"] = r.Tipo,
            ["calificacion"] = r.Calificacion,
            ["fecha_inicio"] = r.FechaInicio,
            ["fecha_fin"] = r.FechaFin
        }));
        using var conexion = Conexion;
        return await conexion.QueryAsync<Acreditacion>(
            "SELECT * FROM sp_crear_acreditacion_lote(@Maestro, @Renglones::jsonb)", new { Maestro = maestro, Renglones = json });
    }
}