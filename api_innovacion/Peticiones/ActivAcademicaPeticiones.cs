using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/activ_academica. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// disenio es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Es opcional (null = «(ninguna)»).
public class CrearActivAcademicaPeticion : IValidatableObject
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

    public int? Disenio { get; set; }

    // D-v2-14: la base exige entidad y país (NOT NULL), pero una actividad que no es
    // espejo no los tiene. Si espejo = 1 son obligatorios; si es 0 el servicio guarda
    // «No aplica». Se valida aquí para que el error sea un 400 normal.
    public IEnumerable<ValidationResult> Validate(ValidationContext contexto)
    {
        if (Espejo == 1 && string.IsNullOrWhiteSpace(EntidadEspejo))
            yield return new ValidationResult("entidad_espejo es obligatorio cuando espejo = 1", new[] { nameof(EntidadEspejo) });
        if (Espejo == 1 && string.IsNullOrWhiteSpace(PaisEspejo))
            yield return new ValidationResult("pais_espejo es obligatorio cuando espejo = 1", new[] { nameof(PaisEspejo) });
    }
}

// Lo que llega en el body de PUT /api/activ_academica/{id}. No lleva la llave
// (id): va en la URL y no se cambia por aquí.
public class ActualizarActivAcademicaPeticion : IValidatableObject
{
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

    public int? Disenio { get; set; }

    // D-v2-14: la base exige entidad y país (NOT NULL), pero una actividad que no es
    // espejo no los tiene. Si espejo = 1 son obligatorios; si es 0 el servicio guarda
    // «No aplica». Se valida aquí para que el error sea un 400 normal.
    public IEnumerable<ValidationResult> Validate(ValidationContext contexto)
    {
        if (Espejo == 1 && string.IsNullOrWhiteSpace(EntidadEspejo))
            yield return new ValidationResult("entidad_espejo es obligatorio cuando espejo = 1", new[] { nameof(EntidadEspejo) });
        if (Espejo == 1 && string.IsNullOrWhiteSpace(PaisEspejo))
            yield return new ValidationResult("pais_espejo es obligatorio cuando espejo = 1", new[] { nameof(PaisEspejo) });
    }
}