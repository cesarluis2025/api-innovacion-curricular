using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAnProgramaRepositorio
{
    Task<IEnumerable<AnPrograma>> ListarAsync();
    Task<AnPrograma?> ObtenerPorLlaveAsync(int aspectoNormativo, int programa);
    Task<AnPrograma> AsignarAsync(AnPrograma entidad);
    Task<bool> RetirarAsync(int aspectoNormativo, int programa);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class AnProgramaRepositorio : IAnProgramaRepositorio
{
    private readonly string _cadenaConexion;
    public AnProgramaRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<AnPrograma>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<AnPrograma>("SELECT * FROM sp_listar_an_programa()");
    }

    public async Task<AnPrograma?> ObtenerPorLlaveAsync(int aspectoNormativo, int programa)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<AnPrograma>(
            "SELECT * FROM sp_consultar_an_programa(@AspectoNormativo, @Programa)", new { AspectoNormativo = aspectoNormativo, Programa = programa });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<AnPrograma> AsignarAsync(AnPrograma entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<AnPrograma>("SELECT * FROM sp_crear_an_programa(@AspectoNormativo, @Programa)", entidad);
    }

    public async Task<bool> RetirarAsync(int aspectoNormativo, int programa)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_an_programa(@AspectoNormativo, @Programa)", new { AspectoNormativo = aspectoNormativo, Programa = programa });
    }
}