using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IProgramaPeRepositorio
{
    Task<IEnumerable<ProgramaPe>> ListarAsync();
    Task<ProgramaPe?> ObtenerPorLlaveAsync(int programa, int practicaEstrategia);
    Task<ProgramaPe> AsignarAsync(ProgramaPe entidad);
    Task<bool> RetirarAsync(int programa, int practicaEstrategia);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class ProgramaPeRepositorio : IProgramaPeRepositorio
{
    private readonly string _cadenaConexion;
    public ProgramaPeRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<ProgramaPe>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<ProgramaPe>("SELECT * FROM sp_listar_programa_pe()");
    }

    public async Task<ProgramaPe?> ObtenerPorLlaveAsync(int programa, int practicaEstrategia)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<ProgramaPe>(
            "SELECT * FROM sp_consultar_programa_pe(@Programa, @PracticaEstrategia)", new { Programa = programa, PracticaEstrategia = practicaEstrategia });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<ProgramaPe> AsignarAsync(ProgramaPe entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<ProgramaPe>("SELECT * FROM sp_crear_programa_pe(@Programa, @PracticaEstrategia)", entidad);
    }

    public async Task<bool> RetirarAsync(int programa, int practicaEstrategia)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_programa_pe(@Programa, @PracticaEstrategia)", new { Programa = programa, PracticaEstrategia = practicaEstrategia });
    }
}