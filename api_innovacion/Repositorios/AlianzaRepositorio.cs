using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAlianzaRepositorio
{
    Task<IEnumerable<Alianza>> ListarAsync();
    Task<Alianza?> ObtenerPorLlaveAsync(int aliado, int departamento);
    Task<Alianza> AsignarAsync(Alianza entidad);
    Task<Alianza?> ActualizarAsync(Alianza entidad);
    Task<bool> RetirarAsync(int aliado, int departamento);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class AlianzaRepositorio : IAlianzaRepositorio
{
    private readonly string _cadenaConexion;
    public AlianzaRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Alianza>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<Alianza>("SELECT * FROM sp_listar_alianza()");
    }

    public async Task<Alianza?> ObtenerPorLlaveAsync(int aliado, int departamento)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Alianza>(
            "SELECT * FROM sp_consultar_alianza(@Aliado, @Departamento)", new { Aliado = aliado, Departamento = departamento });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<Alianza> AsignarAsync(Alianza entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<Alianza>("SELECT * FROM sp_crear_alianza(@Aliado, @Departamento, @FechaInicio::date, @FechaFin::date, @Docente)", entidad);
    }

    // CERO filas si la pareja no existe o está retirada: aquí null, y el servicio da 404.
    public async Task<Alianza?> ActualizarAsync(Alianza entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Alianza>("SELECT * FROM sp_actualizar_alianza(@Aliado, @Departamento, @FechaInicio::date, @FechaFin::date, @Docente)", entidad);
    }

    public async Task<bool> RetirarAsync(int aliado, int departamento)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_alianza(@Aliado, @Departamento)", new { Aliado = aliado, Departamento = departamento });
    }
}