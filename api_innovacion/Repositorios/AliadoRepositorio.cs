using System.Data;
using Dapper;
using Npgsql;
using ApiInnovacionCurricular.Modelos;

namespace ApiInnovacionCurricular.Repositorios;

public interface IAliadoRepositorio
{
    Task<IEnumerable<Aliado>> ListarAsync();
    Task<Aliado?> ObtenerPorNitAsync(int nit);
    Task<bool> ExisteNitAsync(int nit);
    Task CrearAsync(Aliado aliado);
    Task<bool> ActualizarAsync(Aliado aliado);
    Task<bool> EliminarLogicoAsync(int nit);
}

public class AliadoRepositorio : IAliadoRepositorio
{
    private readonly string _cadenaConexion;
    public AliadoRepositorio(string cadenaConexion) => _cadenaConexion = cadenaConexion;
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    public async Task<IEnumerable<Aliado>> ListarAsync()
    {
        const string sql = @"SELECT nit AS ""Nit"", razon_social AS ""RazonSocial"",
                              nombre_contacto AS ""NombreContacto"", correo AS ""Correo"",
                              telefono AS ""Telefono"", ciudad AS ""Ciudad"", activo AS ""Activo""
                              FROM aliado WHERE activo = true ORDER BY nit";
        using var conexion = Conexion;
        return await conexion.QueryAsync<Aliado>(sql);
    }

    public async Task<Aliado?> ObtenerPorNitAsync(int nit)
    {
        const string sql = @"SELECT nit AS ""Nit"", razon_social AS ""RazonSocial"",
                              nombre_contacto AS ""NombreContacto"", correo AS ""Correo"",
                              telefono AS ""Telefono"", ciudad AS ""Ciudad"", activo AS ""Activo""
                              FROM aliado WHERE nit = @Nit AND activo = true";
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Aliado>(sql, new { Nit = nit });
    }

    public async Task<bool> ExisteNitAsync(int nit)
    {
        const string sql = "SELECT COUNT(1) FROM aliado WHERE nit = @Nit";
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<int>(sql, new { Nit = nit }) > 0;
    }

    public async Task CrearAsync(Aliado aliado)
    {
        const string sql = @"INSERT INTO aliado (nit, razon_social, nombre_contacto, correo, telefono, ciudad, activo)
                              VALUES (@Nit, @RazonSocial, @NombreContacto, @Correo, @Telefono, @Ciudad, true)";
        using var conexion = Conexion;
        await conexion.ExecuteAsync(sql, aliado);
    }

    public async Task<bool> ActualizarAsync(Aliado aliado)
    {
        const string sql = @"UPDATE aliado SET razon_social = @RazonSocial, nombre_contacto = @NombreContacto,
                              correo = @Correo, telefono = @Telefono, ciudad = @Ciudad
                              WHERE nit = @Nit AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, aliado) > 0;
    }

    public async Task<bool> EliminarLogicoAsync(int nit)
    {
        const string sql = "UPDATE aliado SET activo = false WHERE nit = @Nit AND activo = true";
        using var conexion = Conexion;
        return await conexion.ExecuteAsync(sql, new { Nit = nit }) > 0;
    }
}
