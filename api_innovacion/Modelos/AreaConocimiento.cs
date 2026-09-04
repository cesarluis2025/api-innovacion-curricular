namespace ApiInnovacionCurricular.Modelos;

// Representa exactamente una fila de la tabla area_conocimiento.
// Dapper llena esta clase automáticamente a partir del resultado del SELECT,
// haciendo match por nombre de columna.
public class AreaConocimiento
{
    public int Id { get; set; }
    public string GranArea { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
