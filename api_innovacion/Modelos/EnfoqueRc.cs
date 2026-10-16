using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla puente enfoque_rc: la llave es la PAREJA (enfoque, registro_calificado),
// no hay un id propio.
public class EnfoqueRc
{
    public int Enfoque { get; set; }
    public int RegistroCalificado { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EnfoqueNombre { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RegistroCalificadoNombre { get; set; }
}