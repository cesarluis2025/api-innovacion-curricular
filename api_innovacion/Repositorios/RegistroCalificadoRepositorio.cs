using System.Data;
using System.Text.Json;
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
    Task<IEnumerable<RegistroCalificado>> CrearLoteAsync(int maestro, IEnumerable<RegistroCalificado> renglones);
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

    // Varios renglones en UNA llamada = UNA transacción (D-v2-5): la función
    // guarda todos o ninguno. El JSON usa los nombres de las COLUMNAS (snake_case)
    // porque así los lee jsonb_to_recordset dentro de la función; se arma a mano
    // y no con una política de nombres para que no dependa de cómo se parta «HAcom».
    public async Task<IEnumerable<RegistroCalificado>> CrearLoteAsync(int maestro, IEnumerable<RegistroCalificado> renglones)
    {
        var json = JsonSerializer.Serialize(renglones.Select(r => new Dictionary<string, object?>
        {
            ["codigo"] = r.Codigo,
            ["cant_creditos"] = r.CantCreditos,
            ["hora_acom"] = r.HoraAcom,
            ["hora_ind"] = r.HoraInd,
            ["metodologia"] = r.Metodologia,
            ["fecha_inicio"] = r.FechaInicio.ToString("yyyy-MM-dd"),
            ["fecha_fin"] = r.FechaFin.ToString("yyyy-MM-dd"),
            ["duracion_anios"] = r.DuracionAnios,
            ["duracion_semestres"] = r.DuracionSemestres,
            ["tipo_titulacion"] = r.TipoTitulacion
        }));
        using var conexion = Conexion;
        return await conexion.QueryAsync<RegistroCalificado>(
            "SELECT * FROM sp_crear_registro_calificado_lote(@Maestro, @Renglones::jsonb)", new { Maestro = maestro, Renglones = json });
    }
}