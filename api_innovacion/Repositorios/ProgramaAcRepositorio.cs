using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IProgramaAcRepositorio
{
    Task<IEnumerable<ProgramaAc>> ListarAsync();
    Task<ProgramaAc?> ObtenerPorLlaveAsync(int programa, int areaConocimiento);
    Task<ProgramaAc> AsignarAsync(ProgramaAc entidad);
    Task<bool> RetirarAsync(int programa, int areaConocimiento);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class ProgramaAcRepositorio : IProgramaAcRepositorio
{
    private readonly string _cadenaConexion;
    public ProgramaAcRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<ProgramaAc>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<ProgramaAc>("SELECT * FROM sp_listar_programa_ac()");
    }

    public async Task<ProgramaAc?> ObtenerPorLlaveAsync(int programa, int areaConocimiento)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<ProgramaAc>(
            "SELECT * FROM sp_consultar_programa_ac(@Programa, @AreaConocimiento)", new { Programa = programa, AreaConocimiento = areaConocimiento });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<ProgramaAc> AsignarAsync(ProgramaAc entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<ProgramaAc>("SELECT * FROM sp_crear_programa_ac(@Programa, @AreaConocimiento)", entidad);
    }

    public async Task<bool> RetirarAsync(int programa, int areaConocimiento)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_programa_ac(@Programa, @AreaConocimiento)", new { Programa = programa, AreaConocimiento = areaConocimiento });
    }
}