using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearRegistroCalificadoPeticion pero SIN el campo del padre
// (programa), porque el maestro viaja una sola vez en la ruta.
public class RenglonRegistroCalificadoPeticion
{
    public int Codigo { get; set; }

    [Required(ErrorMessage = "cant_creditos es obligatorio")]
    [MaxLength(45)]
    public string CantCreditos { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_acom es obligatorio")]
    [MaxLength(45)]
    public string HoraAcom { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_ind es obligatorio")]
    [MaxLength(45)]
    public string HoraInd { get; set; } = string.Empty;

    [Required(ErrorMessage = "metodologia es obligatorio")]
    [MaxLength(45)]
    public string Metodologia { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "fecha_fin es obligatorio")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "duracion_anios es obligatorio")]
    [MaxLength(45)]
    public string DuracionAnios { get; set; } = string.Empty;

    [Required(ErrorMessage = "duracion_semestres es obligatorio")]
    [MaxLength(45)]
    public string DuracionSemestres { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo_titulacion es obligatorio")]
    [MaxLength(45)]
    public string TipoTitulacion { get; set; } = string.Empty;
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LoteRegistroCalificadoPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonRegistroCalificadoPeticion> Renglones { get; set; } = new();
}