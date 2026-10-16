using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/pasantia. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// programa es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Si no existe o está retirada, la base responde 23503 y la API 409.
public class CrearPasantiaPeticion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "pais es obligatorio")]
    [MaxLength(45)]
    public string Pais { get; set; } = string.Empty;

    [Required(ErrorMessage = "empresa es obligatorio")]
    [MaxLength(45)]
    public string Empresa { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;

    public int Programa { get; set; }
}

// Lo que llega en el body de PUT /api/pasantia/{id}. No lleva la llave
// (id): va en la URL y no se cambia por aquí.
public class ActualizarPasantiaPeticion
{
    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "pais es obligatorio")]
    [MaxLength(45)]
    public string Pais { get; set; } = string.Empty;

    [Required(ErrorMessage = "empresa es obligatorio")]
    [MaxLength(45)]
    public string Empresa { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;

    public int Programa { get; set; }
}