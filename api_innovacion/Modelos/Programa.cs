using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla programa. Dapper la llena por nombre de columna
// (razon_social -> RazonSocial, ver MatchNamesWithUnderscores en Program.cs).
public class Programa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public string? FechaCierre { get; set; }
    public string NumeroCohortes { get; set; } = string.Empty;
    public string CantGraduados { get; set; } = string.Empty;
    public string FechaActualizacion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int Facultad { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (el JOIN de sp_listar_/sp_consultar_ trae el
    // nombre del padre para mostrarlo en pantalla). En la respuesta de POST/PUT
    // es null y no se serializa, porque la fila se devuelve como quedó guardada.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FacultadNombre { get; set; }
}