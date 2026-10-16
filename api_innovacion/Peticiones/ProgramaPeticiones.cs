using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/programa. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// // facultad es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Si no existe o está retirada, la base responde 23503 y la API 409.
public class CrearProgramaPeticion
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

    public int Facultad { get; set; }
}

// Lo que llega en el body de PUT /api/programa/{id}. No lleva la llave
// (id): va en la URL y no se cambia por aquí.
public class ActualizarProgramaPeticion
{
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

    public int Facultad { get; set; }
}