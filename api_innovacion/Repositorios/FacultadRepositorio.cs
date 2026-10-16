using System.Data;
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
}