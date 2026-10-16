using System.Data;
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
}