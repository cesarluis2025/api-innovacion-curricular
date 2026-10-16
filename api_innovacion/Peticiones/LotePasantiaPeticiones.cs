using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearPasantiaPeticion pero SIN el campo del padre
// (programa), porque el maestro viaja una sola vez en la ruta.
public class RenglonPasantiaPeticion
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
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LotePasantiaPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonPasantiaPeticion> Renglones { get; set; } = new();
}