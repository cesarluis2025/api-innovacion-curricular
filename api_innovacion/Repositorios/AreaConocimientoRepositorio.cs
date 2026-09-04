using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public class AreaConocimientoRepositorio : IAreaConocimientoRepositorio
{
    private readonly string _cadenaConexion;

    // ASP.NET Core inyecta aquí la cadena de conexión que registramos
    // en Program.cs con builder.Services.AddSingleton(...).
    public AreaConocimientoRepositorio(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    // Cada método abre su propia conexión y la cierra sola (el "using").
    // Con Dapper no se mantiene una conexión abierta compartida.
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<AreaConocimiento>> ListarAsync()
    {
        const string sql = @"
            SELECT id AS ""Id"", gran_area AS ""GranArea"", area AS ""Area"",
                   disciplina AS ""Disciplina"", activo AS ""Activo""
            FROM area_conocimiento
            WHERE activo = true
            ORDER BY id";
        using var conexion = Conexion;
        return await conexion.QueryAsync<AreaConocimiento>(sql);
    }

    public async Task<AreaConocimiento?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"
            SELECT id AS ""Id"", gran_area AS ""GranArea"", area AS ""Area"",
                   disciplina AS ""Disciplina"", activo AS ""Activo""
            FROM area_conocimiento
            WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<AreaConocimiento>(sql, new { Id = id });
    }

    public async Task<bool> ExisteIdAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM area_conocimiento WHERE id = @Id";
        using var conexion = Conexion;
        var cantidad = await conexion.ExecuteScalarAsync<int>(sql, new { Id = id });
        return cantidad > 0;
    }

    public async Task CrearAsync(AreaConocimiento area)
    {
        const string sql = @"
            INSERT INTO area_conocimiento (id, gran_area, area, disciplina, activo)
            VALUES (@Id, @GranArea, @Area, @Disciplina, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, area);
    }

    public async Task<bool> ActualizarAsync(AreaConocimiento area)
    {
        const string sql = @"
            UPDATE area_conocimiento
            SET gran_area = @GranArea, area = @Area, disciplina = @Disciplina
            WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        var filasAfectadas = await conexion.ExecuteAsync(sql, area);
        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        // Esto es el borrado lógico: nunca DELETE, siempre marcar inactivo.
        const string sql = "UPDATE area_conocimiento SET activo = false WHERE id = @Id AND activo = true";
        using var conexion = Conexion;
        var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = id });
        return filasAfectadas > 0;
    }
}
