using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla puente docente_departamento: la llave es la PAREJA (docente, departamento),
// no hay un id propio.
public class DocenteDepartamento
{
    public int Docente { get; set; }
    public int Departamento { get; set; }
    public string Dedicacion { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaSalida { get; set; }
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DepartamentoNombre { get; set; }
}