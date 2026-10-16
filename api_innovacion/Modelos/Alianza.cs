using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla puente alianza: la llave es la PAREJA (aliado, departamento),
// no hay un id propio.
public class Alianza
{
    public int Aliado { get; set; }
    public int Departamento { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? Docente { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AliadoNombre { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DepartamentoNombre { get; set; }
}