using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAaRcRepositorio
{
    Task<IEnumerable<AaRc>> ListarAsync();
    Task<AaRc?> ObtenerPorLlaveAsync(int activAcademicasIdcurso, int registroCalificadoCodigo);
    Task<AaRc> AsignarAsync(AaRc entidad);
    Task<AaRc?> ActualizarAsync(AaRc entidad);
    Task<bool> RetirarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class AaRcRepositorio : IAaRcRepositorio
{
    private readonly string _cadenaConexion;
    public AaRcRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<AaRc>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<AaRc>("SELECT * FROM sp_listar_aa_rc()");
    }

    public async Task<AaRc?> ObtenerPorLlaveAsync(int activAcademicasIdcurso, int registroCalificadoCodigo)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<AaRc>(
            "SELECT * FROM sp_consultar_aa_rc(@ActivAcademicasIdcurso, @RegistroCalificadoCodigo)", new { ActivAcademicasIdcurso = activAcademicasIdcurso, RegistroCalificadoCodigo = registroCalificadoCodigo });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<AaRc> AsignarAsync(AaRc entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<AaRc>("SELECT * FROM sp_crear_aa_rc(@ActivAcademicasIdcurso, @RegistroCalificadoCodigo, @Componente, @Semestre)", entidad);
    }

    // CERO filas si la pareja no existe o está retirada: aquí null, y el servicio da 404.
    public async Task<AaRc?> ActualizarAsync(AaRc entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<AaRc>("SELECT * FROM sp_actualizar_aa_rc(@ActivAcademicasIdcurso, @RegistroCalificadoCodigo, @Componente, @Semestre)", entidad);
    }

    public async Task<bool> RetirarAsync(int activAcademicasIdcurso, int registroCalificadoCodigo)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_aa_rc(@ActivAcademicasIdcurso, @RegistroCalificadoCodigo)", new { ActivAcademicasIdcurso = activAcademicasIdcurso, RegistroCalificadoCodigo = registroCalificadoCodigo });
    }
}