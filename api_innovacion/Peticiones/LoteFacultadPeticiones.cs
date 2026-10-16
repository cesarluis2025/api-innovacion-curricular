using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearFacultadPeticion pero SIN el campo del padre
// (universidad), porque el maestro viaja una sola vez en la ruta.
public class RenglonFacultadPeticion
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
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LoteFacultadPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonFacultadPeticion> Renglones { get; set; } = new();
}