using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IActivAcademicaRepositorio
{
    Task<IEnumerable<ActivAcademica>> ListarAsync();
    Task<ActivAcademica?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task<ActivAcademica> CrearAsync(ActivAcademica entidad);
    Task<ActivAcademica?> ActualizarAsync(ActivAcademica entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

// Solo llama a las funciones sp_ de db/04_sp_v2.sql: la API no escribe SQL de
// tablas. Las fechas llevan ::date porque Npgsql envía un DateTime como
// timestamp, y la función espera date (no hay conversión implícita).
public class ActivAcademicaRepositorio : IActivAcademicaRepositorio
{
    private readonly string _cadenaConexion;
    public ActivAcademicaRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<ActivAcademica>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<ActivAcademica>("SELECT * FROM sp_listar_activ_academica()");
    }

    public async Task<ActivAcademica?> ObtenerPorIdAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<ActivAcademica>(
            "SELECT * FROM sp_consultar_activ_academica(@Id)", new { Id = id });
    }

    // Solo ve registros ACTIVOS: si la llave existe pero está retirada, el
    // INSERT falla con 23505 y el manejador de errores responde 409.
    public async Task<bool> ExisteIdAsync(int id)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<ActivAcademica>(
            "SELECT * FROM sp_consultar_activ_academica(@Id)", new { Id = id });
        return fila is not null;
    }

    // Devuelve la fila como quedó guardada (valores por defecto incluidos),
    // que es lo que el contrato pide en el 201.
    public async Task<ActivAcademica> CrearAsync(ActivAcademica entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<ActivAcademica>("SELECT * FROM sp_crear_activ_academica(@Id, @Nombre, @NumCreditos, @Tipo, @AreaFormacion, @HAcom, @HIndep, @Idioma, @Espejo, @EntidadEspejo, @PaisEspejo, @Disenio)", entidad);
    }

    // La función devuelve CERO filas si la llave no existe o está retirada:
    // aquí eso es null y el servicio lo traduce a 404.
    public async Task<ActivAcademica?> ActualizarAsync(ActivAcademica entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<ActivAcademica>("SELECT * FROM sp_actualizar_activ_academica(@Id, @Nombre, @NumCreditos, @Tipo, @AreaFormacion, @HAcom, @HIndep, @Idioma, @Espejo, @EntidadEspejo, @PaisEspejo, @Disenio)", entidad);
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_activ_academica(@Id)", new { Id = id });
    }
}