using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/facultad. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// universidad es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Si no existe o está retirada, la base responde 23503 y la API 409.
public class CrearFacultadPeticion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(60)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_fun es obligatorio")]
    public DateTime FechaFun { get; set; }

    public int Universidad { get; set; }
}

// Lo que llega en el body de PUT /api/facultad/{id}. No lleva la llave
// (id): va en la URL y no se cambia por aquí.
public class ActualizarFacultadPeticion
{
    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(60)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_fun es obligatorio")]
    public DateTime FechaFun { get; set; }

    public int Universidad { get; set; }
}