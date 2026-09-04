using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IPracticaEstrategiaRepositorio
{
    Task<IEnumerable<PracticaEstrategia>> ListarAsync();
    Task<PracticaEstrategia?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(PracticaEstrategia entidad);
    Task<bool> ActualizarAsync(PracticaEstrategia entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class PracticaEstrategiaRepositorio : IPracticaEstrategiaRepositorio
{
    private readonly string _cadenaConexion;
    public PracticaEstrategiaRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<PracticaEstrategia>> ListarAsync()
    {
        const string sql = @"SELECT id AS ""Id"", tipo AS ""Tipo"", nombre AS ""Nombre"",
                              descripcion AS ""Descripcion"", activo AS ""Activo""
                              FROM practica_estrategia WHERE activo = true ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<PracticaEstrategia>(sql);
    }

    public async Task<PracticaEstrategia?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id AS ""Id"", tipo AS ""Tipo"", nombre AS ""Nombre"",
                              descripcion AS ""Descripcion"", activo AS ""Activo""
                              FROM practica_estrategia WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<PracticaEstrategia>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM practica_estrategia WHERE id = @Id";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Id = id }) > 0;
    }

    public async Task CrearAsync(PracticaEstrategia entidad)
    {
        const string sql = @"INSERT INTO practica_estrategia (id, tipo, nombre, descripcion, activo)
                              VALUES (@Id, @Tipo, @Nombre, @Descripcion, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<bool> ActualizarAsync(PracticaEstrategia entidad)
    {
        const string sql = @"UPDATE practica_estrategia SET tipo = @Tipo, nombre = @Nombre, descripcion = @Descripcion
                              WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, entidad) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        const string sql = "UPDATE practica_estrategia SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
