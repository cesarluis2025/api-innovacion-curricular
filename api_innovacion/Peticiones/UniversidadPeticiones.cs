using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

public class CrearUniversidadPeticion
{
    [Required(ErrorMessage = "el id es obligatorio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(60)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "ciudad es obligatorio")]
    [MaxLength(45)]
    public string Ciudad { get; set; } = string.Empty;
}

public class ActualizarUniversidadPeticion
{
    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(60)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "ciudad es obligatorio")]
    [MaxLength(45)]
    public string Ciudad { get; set; } = string.Empty;
}
