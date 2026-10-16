using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla registro_calificado. Dapper la llena por nombre de columna
// (razon_social -> RazonSocial, ver MatchNamesWithUnderscores en Program.cs).
public class RegistroCalificado
{
    public int Codigo { get; set; }
    public string CantCreditos { get; set; } = string.Empty;
    public string HoraAcom { get; set; } = string.Empty;
    public string HoraInd { get; set; } = string.Empty;
    public string Metodologia { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string DuracionAnios { get; set; } = string.Empty;
    public string DuracionSemestres { get; set; } = string.Empty;
    public string TipoTitulacion { get; set; } = string.Empty;
    public int Programa { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (el JOIN de sp_listar_/sp_consultar_ trae el
    // nombre del padre para mostrarlo en pantalla). En la respuesta de POST/PUT
    // es null y no se serializa, porque la fila se devuelve como quedó guardada.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProgramaNombre { get; set; }
}