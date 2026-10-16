using System.Text.Json;
using Npgsql;
using ApiInnovacionCurricular.Excepciones;

namespace ApiInnovacionCurricular.Middleware;

// Un solo lugar donde cualquier error de la API se convierte en una respuesta
// con código HTTP y mensaje en castellano. Sin esto, cada controlador repetiría
// sus try/catch y un error no previsto saldría como 500 con la traza completa.
//
// Mapa (6_contracts.md):
//   NoEncontradoExcepcion        -> 404
//   ConflictoExcepcion           -> 409  (regla de negocio: llave repetida, maestro con detalle)
//   PostgresException 23503/23505 -> 409 (llave foránea inválida / llave repetida que detecta la base)
//   cualquier otra               -> 500 con mensaje genérico (el detalle va al log, no al cliente)
public class ManejadorErrores
{
    // Tablas puente: su llave primaria es la pareja, así que "repetida" se dice
    // como asignación y no como registro.
    private static readonly HashSet<string> TablasPuente = new()
    {
        "programa_ac", "programa_pe", "programa_ci", "an_programa",
        "enfoque_rc", "aa_rc", "docente_departamento", "alianza"
    };

    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorErrores> _log;

    public ManejadorErrores(RequestDelegate siguiente, ILogger<ManejadorErrores> log)
    {
        _siguiente = siguiente;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (NoEncontradoExcepcion ex)
        {
            await Responder(contexto, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictoExcepcion ex)
        {
            await Responder(contexto, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (PostgresException ex) when (ex.SqlState is "23503" or "23505")
        {
            await Responder(contexto, StatusCodes.Status409Conflict, MensajeDeConflicto(ex));
        }
        catch (Exception ex)
        {
            // El cliente no debe ver la traza ni el SQL: solo queda en el log.
            _log.LogError(ex, "Error no previsto en {Metodo} {Ruta}", contexto.Request.Method, contexto.Request.Path);
            await Responder(contexto, StatusCodes.Status500InternalServerError, "ocurrió un error inesperado en el servidor");
        }
    }

    // Las funciones sp_ lanzan sus propios 23503/23505 con el mensaje YA en
    // castellano («el campo facultad no corresponde…», «renglón 2: …»): se
    // entregan tal cual. Los errores nativos de PostgreSQL llegan en inglés
    // («duplicate key value…») y se reescriben con el nombre de la tabla.
    private static string MensajeDeConflicto(PostgresException ex)
    {
        var nativo = ex.MessageText.StartsWith("duplicate key", StringComparison.OrdinalIgnoreCase)
                  || ex.MessageText.StartsWith("insert or update", StringComparison.OrdinalIgnoreCase)
                  || ex.MessageText.StartsWith("update or delete", StringComparison.OrdinalIgnoreCase);
        if (!nativo) return ex.MessageText;

        if (ex.SqlState == "23505")
        {
            var tabla = ex.TableName ?? "";
            return TablasPuente.Contains(tabla)
                ? "la asignación ya existe"
                : $"ya existe un registro de {tabla.Replace('_', ' ')} con esa llave";
        }
        return "el registro se relaciona con otro que no existe o no está activo";
    }

    private static Task Responder(HttpContext contexto, int codigo, string mensaje)
    {
        contexto.Response.StatusCode = codigo;
        contexto.Response.ContentType = "application/json; charset=utf-8";
        return contexto.Response.WriteAsync(JsonSerializer.Serialize(new { mensaje }));
    }
}