using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IEnfoqueRcRepositorio
{
    Task<IEnumerable<EnfoqueRc>> ListarAsync();
    Task<EnfoqueRc?> ObtenerPorLlaveAsync(int enfoque, int registroCalificado);
    Task<EnfoqueRc> AsignarAsync(EnfoqueRc entidad);
    Task<bool> RetirarAsync(int enfoque, int registroCalificado);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class EnfoqueRcRepositorio : IEnfoqueRcRepositorio
{
    private readonly string _cadenaConexion;
    public EnfoqueRcRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<EnfoqueRc>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<EnfoqueRc>("SELECT * FROM sp_listar_enfoque_rc()");
    }

    public async Task<EnfoqueRc?> ObtenerPorLlaveAsync(int enfoque, int registroCalificado)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<EnfoqueRc>(
            "SELECT * FROM sp_consultar_enfoque_rc(@Enfoque, @RegistroCalificado)", new { Enfoque = enfoque, RegistroCalificado = registroCalificado });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<EnfoqueRc> AsignarAsync(EnfoqueRc entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<EnfoqueRc>("SELECT * FROM sp_crear_enfoque_rc(@Enfoque, @RegistroCalificado)", entidad);
    }

    public async Task<bool> RetirarAsync(int enfoque, int registroCalificado)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_enfoque_rc(@Enfoque, @RegistroCalificado)", new { Enfoque = enfoque, RegistroCalificado = registroCalificado });
    }
}