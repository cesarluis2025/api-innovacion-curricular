using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAspectoNormativoRepositorio
{
    Task<IEnumerable<AspectoNormativo>> ListarAsync();
    Task<AspectoNormativo?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(AspectoNormativo entidad);
    Task<bool> ActualizarAsync(AspectoNormativo entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class AspectoNormativoRepositorio : IAspectoNormativoRepositorio
{
    private readonly string _cadenaConexion;
    public AspectoNormativoRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<AspectoNormativo>> ListarAsync()
    {
        const string sql = @"SELECT id AS ""Id"", tipo AS ""Tipo"", descripcion AS ""Descripcion"",
                              fuente AS ""Fuente"", activo AS ""Activo""
                              FROM aspecto_normativo WHERE activo = true ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<AspectoNormativo>(sql);
    }

    public async Task<AspectoNormativo?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id AS ""Id"", tipo AS ""Tipo"", descripcion AS ""Descripcion"",
                              fuente AS ""Fuente"", activo AS ""Activo""
                              FROM aspecto_normativo WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<AspectoNormativo>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM aspecto_normativo WHERE id = @Id";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Id = id }) > 0;
    }

    public async Task CrearAsync(AspectoNormativo entidad)
    {
        const string sql = @"INSERT INTO aspecto_normativo (id, tipo, descripcion, fuente, activo)
                              VALUES (@Id, @Tipo, @Descripcion, @Fuente, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<bool> ActualizarAsync(AspectoNormativo entidad)
    {
        const string sql = @"UPDATE aspecto_normativo SET tipo = @Tipo, descripcion = @Descripcion, fuente = @Fuente
                              WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, entidad) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        const string sql = "UPDATE aspecto_normativo SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
