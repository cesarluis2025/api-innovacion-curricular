using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearActivAcademicaPeticion pero SIN el campo del padre
// (disenio), porque el maestro viaja una sola vez en la ruta.
public class RenglonActivAcademicaPeticion : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    public int NumCreditos { get; set; }

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "area_formacion es obligatorio")]
    [MaxLength(45)]
    public string AreaFormacion { get; set; } = string.Empty;

    public int HAcom { get; set; }

    public int HIndep { get; set; }

    [Required(ErrorMessage = "idioma es obligatorio")]
    [MaxLength(45)]
    public string Idioma { get; set; } = string.Empty;

    [Range(0, 1, ErrorMessage = "espejo debe ser 0 o 1")]
    public int Espejo { get; set; }

    [MaxLength(45)]
    public string? EntidadEspejo { get; set; }

    [MaxLength(45)]
    public string? PaisEspejo { get; set; }

    // D-v2-14: con espejo = 1 la entidad y el país son obligatorios; con 0 se guarda «No aplica».
    public IEnumerable<ValidationResult> Validate(ValidationContext contexto)
    {
        if (Espejo == 1 && string.IsNullOrWhiteSpace(EntidadEspejo))
            yield return new ValidationResult("entidad_espejo es obligatorio cuando espejo = 1", new[] { nameof(EntidadEspejo) });
        if (Espejo == 1 && string.IsNullOrWhiteSpace(PaisEspejo))
            yield return new ValidationResult("pais_espejo es obligatorio cuando espejo = 1", new[] { nameof(PaisEspejo) });
    }
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LoteActivAcademicaPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonActivAcademicaPeticion> Renglones { get; set; } = new();
}