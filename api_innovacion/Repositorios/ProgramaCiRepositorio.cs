using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IProgramaCiRepositorio
{
    Task<IEnumerable<ProgramaCi>> ListarAsync();
    Task<ProgramaCi?> ObtenerPorLlaveAsync(int programa, int carInnovacion);
    Task<ProgramaCi> AsignarAsync(ProgramaCi entidad);
    Task<bool> RetirarAsync(int programa, int carInnovacion);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class ProgramaCiRepositorio : IProgramaCiRepositorio
{
    private readonly string _cadenaConexion;
    public ProgramaCiRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<ProgramaCi>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<ProgramaCi>("SELECT * FROM sp_listar_programa_ci()");
    }

    public async Task<ProgramaCi?> ObtenerPorLlaveAsync(int programa, int carInnovacion)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<ProgramaCi>(
            "SELECT * FROM sp_consultar_programa_ci(@Programa, @CarInnovacion)", new { Programa = programa, CarInnovacion = carInnovacion });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<ProgramaCi> AsignarAsync(ProgramaCi entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<ProgramaCi>("SELECT * FROM sp_crear_programa_ci(@Programa, @CarInnovacion)", entidad);
    }

    public async Task<bool> RetirarAsync(int programa, int carInnovacion)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_programa_ci(@Programa, @CarInnovacion)", new { Programa = programa, CarInnovacion = carInnovacion });
    }
}