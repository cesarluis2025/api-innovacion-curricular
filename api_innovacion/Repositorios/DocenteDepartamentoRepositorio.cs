using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IDocenteDepartamentoRepositorio
{
    Task<IEnumerable<DocenteDepartamento>> ListarAsync();
    Task<DocenteDepartamento?> ObtenerPorLlaveAsync(int docente, int departamento);
    Task<DocenteDepartamento> AsignarAsync(DocenteDepartamento entidad);
    Task<DocenteDepartamento?> ActualizarAsync(DocenteDepartamento entidad);
    Task<bool> RetirarAsync(int docente, int departamento);
}

// Solo llama a funciones sp_ de db/04_sp_v2_puentes_lotes.sql. La función de
// asignar decide por su cuenta si inserta o reactiva una pareja retirada.
public class DocenteDepartamentoRepositorio : IDocenteDepartamentoRepositorio
{
    private readonly string _cadenaConexion;
    public DocenteDepartamentoRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<DocenteDepartamento>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<DocenteDepartamento>("SELECT * FROM sp_listar_docente_departamento()");
    }

    public async Task<DocenteDepartamento?> ObtenerPorLlaveAsync(int docente, int departamento)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>(
            "SELECT * FROM sp_consultar_docente_departamento(@Docente, @Departamento)", new { Docente = docente, Departamento = departamento });
    }

    // Devuelve la fila como quedó guardada; el 23505 de una pareja activa repetida
    // sube hasta el manejador de errores, que lo vuelve 409.
    public async Task<DocenteDepartamento> AsignarAsync(DocenteDepartamento entidad)
    {
        using var conexion = Conexion;
        return await conexion.QuerySingleAsync<DocenteDepartamento>("SELECT * FROM sp_crear_docente_departamento(@Docente, @Departamento, @Dedicacion, @Modalidad, @FechaIngreso::date, @FechaSalida::date)", entidad);
    }

    // CERO filas si la pareja no existe o está retirada: aquí null, y el servicio da 404.
    public async Task<DocenteDepartamento?> ActualizarAsync(DocenteDepartamento entidad)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<DocenteDepartamento>("SELECT * FROM sp_actualizar_docente_departamento(@Docente, @Departamento, @Dedicacion, @Modalidad, @FechaIngreso::date, @FechaSalida::date)", entidad);
    }

    public async Task<bool> RetirarAsync(int docente, int departamento)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_docente_departamento(@Docente, @Departamento)", new { Docente = docente, Departamento = departamento });
    }
}