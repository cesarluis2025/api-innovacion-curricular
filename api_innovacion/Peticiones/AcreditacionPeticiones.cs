using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/acreditacion. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// // programa es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Si no existe o está retirada, la base responde 23503 y la API 409.
public class CrearAcreditacionPeticion
{
    public int Resolucion { get; set; }

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "calificacion es obligatorio")]
    [MaxLength(45)]
    public string Calificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    [MaxLength(45)]
    public string FechaInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_fin es obligatorio")]
    [MaxLength(45)]
    public string FechaFin { get; set; } = string.Empty;

    public int Programa { get; set; }
}

// Lo que llega en el body de PUT /api/acreditacion/{resolucion}. No lleva la llave
// (resolucion): va en la URL y no se cambia por aquí.
public class ActualizarAcreditacionPeticion
{
    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "calificacion es obligatorio")]
    [MaxLength(45)]
    public string Calificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    [MaxLength(45)]
    public string FechaInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_fin es obligatorio")]
    [MaxLength(45)]
    public string FechaFin { get; set; } = string.Empty;

    public int Programa { get; set; }
}