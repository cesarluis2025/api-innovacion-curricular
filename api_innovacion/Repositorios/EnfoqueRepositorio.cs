using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IEnfoqueRepositorio
{
    Task<IEnumerable<Enfoque>> ListarAsync();
    Task<Enfoque?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(Enfoque entidad);
    Task<bool> ActualizarAsync(Enfoque entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class EnfoqueRepositorio : IEnfoqueRepositorio
{
    private readonly string _cadenaConexion;
    public EnfoqueRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Enfoque>> ListarAsync()
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", descripcion AS ""Descripcion"", activo AS ""Activo""
                              FROM enfoque WHERE activo = true ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<Enfoque>(sql);
    }

    public async Task<Enfoque?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", descripcion AS ""Descripcion"", activo AS ""Activo""
                              FROM enfoque WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Enfoque>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM enfoque WHERE id = @Id";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Id = id }) > 0;
    }

    public async Task CrearAsync(Enfoque entidad)
    {
        const string sql = "INSERT INTO enfoque (id, nombre, descripcion, activo) VALUES (@Id, @Nombre, @Descripcion, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<bool> ActualizarAsync(Enfoque entidad)
    {
        const string sql = "UPDATE enfoque SET nombre = @Nombre, descripcion = @Descripcion WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, entidad) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        const string sql = "UPDATE enfoque SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
