using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

public class CrearAspectoNormativoPeticion
{
    [Required(ErrorMessage = "el id es obligatorio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "fuente es obligatorio")]
    [MaxLength(45)]
    public string Fuente { get; set; } = string.Empty;
}

public class ActualizarAspectoNormativoPeticion
{
    [Required(ErrorMessage = "tipo es obligatorio")]
    [MaxLength(45)]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "fuente es obligatorio")]
    [MaxLength(45)]
    public string Fuente { get; set; } = string.Empty;
}
