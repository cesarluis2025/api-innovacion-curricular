using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

public class CrearEnfoquePeticion
{
    [Required(ErrorMessage = "el id es obligatorio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;
}

public class ActualizarEnfoquePeticion
{
    [Required(ErrorMessage = "nombre es obligatorio")]
    [MaxLength(45)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "descripcion es obligatorio")]
    [MaxLength(45)]
    public string Descripcion { get; set; } = string.Empty;
}
