using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/activ_academica. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// // disenio es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Es opcional (null = «(ninguna)»).
public class CrearActivAcademicaPeticion
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

    public int Espejo { get; set; }

    [Required(ErrorMessage = "entidad_espejo es obligatorio")]
    [MaxLength(45)]
    public string EntidadEspejo { get; set; } = string.Empty;

    [Required(ErrorMessage = "pais_espejo es obligatorio")]
    [MaxLength(45)]
    public string PaisEspejo { get; set; } = string.Empty;

    public int? Disenio { get; set; }
}

// Lo que llega en el body de PUT /api/activ_academica/{id}. No lleva la llave
// (id): va en la URL y no se cambia por aquí.
public class ActualizarActivAcademicaPeticion
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

    public int Espejo { get; set; }

    [Required(ErrorMessage = "entidad_espejo es obligatorio")]
    [MaxLength(45)]
    public string EntidadEspejo { get; set; } = string.Empty;

    [Required(ErrorMessage = "pais_espejo es obligatorio")]
    [MaxLength(45)]
    public string PaisEspejo { get; set; } = string.Empty;

    public int? Disenio { get; set; }
}