using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IUniversidadRepositorio
{
    Task<IEnumerable<Universidad>> ListarAsync();
    Task<Universidad?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(Universidad universidad);
    Task<bool> ActualizarAsync(Universidad universidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class UniversidadRepositorio : IUniversidadRepositorio
{
    private readonly string _cadenaConexion;
    public UniversidadRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Universidad>> ListarAsync()
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", tipo AS ""Tipo"",
                              ciudad AS ""Ciudad"", activo AS ""Activo""
                              FROM universidad WHERE activo = true ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<Universidad>(sql);
    }

    public async Task<Universidad?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", tipo AS ""Tipo"",
                              ciudad AS ""Ciudad"", activo AS ""Activo""
                              FROM universidad WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Universidad>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM universidad WHERE id = @Id";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Id = id }) > 0;
    }

    public async Task CrearAsync(Universidad universidad)
    {
        const string sql = @"INSERT INTO universidad (id, nombre, tipo, ciudad, activo)
                              VALUES (@Id, @Nombre, @Tipo, @Ciudad, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, universidad);
    }

    public async Task<bool> ActualizarAsync(Universidad universidad)
    {
        const string sql = @"UPDATE universidad SET nombre = @Nombre, tipo = @Tipo, ciudad = @Ciudad
                              WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, universidad) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        const string sql = "UPDATE universidad SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
