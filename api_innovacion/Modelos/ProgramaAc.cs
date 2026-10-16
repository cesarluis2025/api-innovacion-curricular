using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla puente programa_ac: la llave es la PAREJA (programa, area_conocimiento),
// no hay un id propio.
public class ProgramaAc
{
    public int Programa { get; set; }
    public int AreaConocimiento { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProgramaNombre { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AreaConocimientoNombre { get; set; }
}