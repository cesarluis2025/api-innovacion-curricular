using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla activ_academica. Dapper la llena por nombre de columna
// (razon_social -> RazonSocial, ver MatchNamesWithUnderscores en Program.cs).
public class ActivAcademica
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int NumCreditos { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string AreaFormacion { get; set; } = string.Empty;
    public int HAcom { get; set; }
    public int HIndep { get; set; }
    public string Idioma { get; set; } = string.Empty;
    public int Espejo { get; set; }
    public string EntidadEspejo { get; set; } = string.Empty;
    public string PaisEspejo { get; set; } = string.Empty;
    public int? Disenio { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (el JOIN de sp_listar_/sp_consultar_ trae el
    // nombre del padre para mostrarlo en pantalla). En la respuesta de POST/PUT
    // es null y no se serializa, porque la fila se devuelve como quedó guardada.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DisenioNombre { get; set; }
}