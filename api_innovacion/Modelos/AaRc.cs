using System.Text.Json.Serialization;

namespace ApiInnovacionCurricular.Modelos;

// Una fila de la tabla puente aa_rc: la llave es la PAREJA (activ_academicas_idcurso, registro_calificado_codigo),
// no hay un id propio.
public class AaRc
{
    public int ActivAcademicasIdcurso { get; set; }
    public int RegistroCalificadoCodigo { get; set; }
    public string Componente { get; set; } = string.Empty;
    public string Semestre { get; set; } = string.Empty;
    public bool Activo { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ActivAcademicaNombre { get; set; }

    // Solo viene en las LECTURAS (JOIN de sp_listar_/sp_consultar_); en la respuesta
    // de POST/PUT es null y no se serializa.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RegistroCalificadoNombre { get; set; }
}