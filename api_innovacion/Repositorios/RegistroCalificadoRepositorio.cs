using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IRegistroCalificadoRepositorio
{
    Task<IEnumerable<RegistroCalificado>> ListarAsync();
    Task<RegistroCalificado?> ObtenerPorIdAsync(int codigo);
    Task<bool> ExisteIdAsync(int codigo);
    Task<RegistroCalificado> CrearAsync(RegistroCalificado entidad);
    Task<RegistroCalificado?> ActualizarAsync(RegistroCalificado entidad);
    Task<bool> EliminarLogicoAsync(int codigo);
}

// Solo llama a las funciones sp_ de db/04_sp_v2.sql: la API no escribe SQL de
// tablas. Las fechas llevan ::date porque Npgsql envía un DateTime como
// timestamp, y la función espera date (no hay conversión implícita).
public class RegistroCalificadoRepositorio : IRegistroCalificadoRepositorio
{
    private readonly string _cadenaConexion;
    public RegistroCalificadoRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<RegistroCalificado>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<RegistroCalificado>("SELECT * FROM sp_listar_registro_calificado()");
    }

    public async Task<RegistroCalificado?> ObtenerPorIdAsync(int codigo)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<RegistroCalificado>(
            "SELECT * FROM sp_consultar_registro_calificado(@Codigo)", new { Codigo = codigo });
    }

    // Solo ve registros ACTIVOS: si la llave existe pero está retirada, el
    // INSERT falla con 23505 y el manejador de errores responde 409.
    public async Task<bool> ExisteIdAsync(int codigo)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<RegistroCalificado>(
            "SELECT * FROM sp_consultar_registro_calificado(@Codigo)", new { Codigo = codigo });
        return fila is not null;
    }

    // Devuelve la fila como quedó guardada (valores por defecto incluidos),
    // que es lo que el contrato pide en el 201.
    public async Task<RegistroCalificado> CrearAsync(RegistroCalificado entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<RegistroCalificado>("SELECT * FROM sp_crear_registro_calificado(@Codigo, @CantCreditos, @HoraAcom, @HoraInd, @Metodologia, @FechaInicio::date, @FechaFin::date, @DuracionAnios, @DuracionSemestres, @TipoTitulacion, @Programa)", entidad);
    }

    // La función devuelve CERO filas si la llave no existe o está retirada:
    // aquí eso es null y el servicio lo traduce a 404.
    public async Task<RegistroCalificado?> ActualizarAsync(RegistroCalificado entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<RegistroCalificado>("SELECT * FROM sp_actualizar_registro_calificado(@Codigo, @CantCreditos, @HoraAcom, @HoraInd, @Metodologia, @FechaInicio::date, @FechaFin::date, @DuracionAnios, @DuracionSemestres, @TipoTitulacion, @Programa)", entidad);
    }

    public async Task<bool> EliminarLogicoAsync(int codigo)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_registro_calificado(@Codigo)", new { Codigo = codigo });
    }
}