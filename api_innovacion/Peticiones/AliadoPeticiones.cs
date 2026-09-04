using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

public class CrearAliadoPeticion
{
    [Required(ErrorMessage = "el nit es obligatorio")]
    public int Nit { get; set; }

    [Required(ErrorMessage = "razon_social es obligatorio")]
    [MaxLength(60)]
    public string RazonSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "nombre_contacto es obligatorio")]
    [MaxLength(60)]
    public string NombreContacto { get; set; } = string.Empty;

    [Required(ErrorMessage = "correo es obligatorio")]
    [EmailAddress(ErrorMessage = "el correo no tiene un formato válido")]
    [MaxLength(70)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "telefono es obligatorio")]
    [MaxLength(45)]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "ciudad es obligatorio")]
    [MaxLength(45)]
    public string Ciudad { get; set; } = string.Empty;
}

public class ActualizarAliadoPeticion
{
    [Required(ErrorMessage = "razon_social es obligatorio")]
    [MaxLength(60)]
    public string RazonSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "nombre_contacto es obligatorio")]
    [MaxLength(60)]
    public string NombreContacto { get; set; } = string.Empty;

    [Required(ErrorMessage = "correo es obligatorio")]
    [EmailAddress(ErrorMessage = "el correo no tiene un formato válido")]
    [MaxLength(70)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "telefono es obligatorio")]
    [MaxLength(45)]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "ciudad es obligatorio")]
    [MaxLength(45)]
    public string Ciudad { get; set; } = string.Empty;
}
