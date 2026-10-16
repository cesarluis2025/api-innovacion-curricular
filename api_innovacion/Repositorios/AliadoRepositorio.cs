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

    // Cada método abre su conexión y la cierra sola (el "using").
    private IDbConnection Conexion => new NpgsqlConnection(_cadenaConexion);

    // Desde la v2 el repositorio NO escribe SQL de tablas: solo llama a las
    // funciones sp_ de db/03_sp_v1.sql. Así la regla (filtrar activos, no
    // borrar de verdad) vive en un único lugar, la base. Las columnas llegan
    // en snake_case y Dapper las une a las propiedades gracias a
    // MatchNamesWithUnderscores (Program.cs).

    public async Task<IEnumerable<Aliado>> ListarAsync()
    {
        using var conexion = Conexion;
        return await conexion.QueryAsync<Aliado>("SELECT * FROM sp_listar_aliado()");
    }

    public async Task<Aliado?> ObtenerPorNitAsync(int nit)
    {
        using var conexion = Conexion;
        return await conexion.QueryFirstOrDefaultAsync<Aliado>(
            "SELECT * FROM sp_consultar_aliado(@Nit)", new { Nit = nit });
    }

    // Solo ve registros ACTIVOS. Si el id existe pero está retirado, el
    // servicio no lo detecta aquí y el INSERT falla con 23505, que el
    // manejador de errores convierte en 409 (nunca se reutiliza una llave).
    public async Task<bool> ExisteNitAsync(int nit)
    {
        using var conexion = Conexion;
        var fila = await conexion.QueryFirstOrDefaultAsync<Aliado>(
            "SELECT * FROM sp_consultar_aliado(@Nit)", new { Nit = nit });
        return fila is not null;
    }

    // La función devuelve la fila insertada; aquí no se necesita, el servicio
    // ya conoce los valores.
    public async Task CrearAsync(Aliado entidad)
    {
        using var conexion = Conexion;
        await conexion.ExecuteAsync("SELECT * FROM sp_crear_aliado(@Nit, @RazonSocial, @NombreContacto, @Correo, @Telefono, @Ciudad)", entidad);
    }

    // La función de actualizar devuelve CERO filas si el id no existe o está
    // retirado; eso es lo que el servicio traduce a 404.
    public async Task<bool> ActualizarAsync(Aliado entidad)
    {
        using var conexion = Conexion;
        var filas = await conexion.QueryAsync<Aliado>("SELECT * FROM sp_actualizar_aliado(@Nit, @RazonSocial, @NombreContacto, @Correo, @Telefono, @Ciudad)", entidad);
        return filas.Any();
    }

    public async Task<bool> EliminarLogicoAsync(int nit)
    {
        using var conexion = Conexion;
        return await conexion.ExecuteScalarAsync<bool>(
            "SELECT * FROM sp_eliminar_aliado(@Nit)", new { Nit = nit });
    }
}