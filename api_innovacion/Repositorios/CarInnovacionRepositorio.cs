using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface ICarInnovacionRepositorio
{
    Task<IEnumerable<CarInnovacion>> ListarAsync();
    Task<CarInnovacion?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteIdAsync(int id);
    Task CrearAsync(CarInnovacion entidad);
    Task<bool> ActualizarAsync(CarInnovacion entidad);
    Task<bool> EliminarLogicoAsync(int id);
}

public class CarInnovacionRepositorio : ICarInnovacionRepositorio
{
    private readonly string _cadenaConexion;
    public CarInnovacionRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<CarInnovacion>> ListarAsync()
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", descripcion AS ""Descripcion"",
                              tipo AS ""Tipo"", activo AS ""Activo""
                              FROM car_innovacion WHERE activo = true ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<CarInnovacion>(sql);
    }

    public async Task<CarInnovacion?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id AS ""Id"", nombre AS ""Nombre"", descripcion AS ""Descripcion"",
                              tipo AS ""Tipo"", activo AS ""Activo""
                              FROM car_innovacion WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<CarInnovacion>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM car_innovacion WHERE id = @Id";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Id = id }) > 0;
    }

    public async Task CrearAsync(CarInnovacion entidad)
    {
        const string sql = @"INSERT INTO car_innovacion (id, nombre, descripcion, tipo, activo)
                              VALUES (@Id, @Nombre, @Descripcion, @Tipo, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<bool> ActualizarAsync(CarInnovacion entidad)
    {
        const string sql = @"UPDATE car_innovacion SET nombre = @Nombre, descripcion = @Descripcion, tipo = @Tipo
                              WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, entidad) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        const string sql = "UPDATE car_innovacion SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
