using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearProgramaPeticion pero SIN el campo del padre
// (facultad), porque el maestro viaja una sola vez en la ruta.
public class RenglonProgramaPeticion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "nivel es obligatorio")]
    [MaxLength(45)]
    public string Nivel { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_creacion es obligatorio")]
    [MaxLength(45)]
    public string FechaCreacion { get; set; } = string.Empty;

    [MaxLength(45)]
    public string? FechaCierre { get; set; }

    [Required(ErrorMessage = "numero_cohortes es obligatorio")]
    [MaxLength(45)]
    public string NumeroCohortes { get; set; } = string.Empty;

    [Required(ErrorMessage = "cant_graduados es obligatorio")]
    [MaxLength(45)]
    public string CantGraduados { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_actualizacion es obligatorio")]
    [MaxLength(45)]
    public string FechaActualizacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "ciudad es obligatorio")]
    [MaxLength(45)]
    public string Ciudad { get; set; } = string.Empty;
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LoteProgramaPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonProgramaPeticion> Renglones { get; set; } = new();
}