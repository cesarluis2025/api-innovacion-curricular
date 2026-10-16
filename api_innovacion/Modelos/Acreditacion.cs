using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla acreditacion. Dapper la llena por nombre de columna
// (razon_social -> RazonSocial, ver MatchNamesWithUnderscores en Program.cs).
public class Acreditacion
{
    public int Resolucion { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Calificacion { get; set; } = string.Empty;
    public string FechaInicio { get; set; } = string.Empty;
    public string FechaFin { get; set; } = string.Empty;
    public int Programa { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (el JOIN de sp_listar_/sp_consultar_ trae el
    // nombre del padre para mostrarlo en pantalla). En la respuesta de POST/PUT
    // es null y no se serializa, porque la fila se devuelve como quedó guardada.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProgramaNombre { get; set; }
}