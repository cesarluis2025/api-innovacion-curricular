using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Un renglón del lote: igual que CrearAcreditacionPeticion pero SIN el campo del padre
// (programa), porque el maestro viaja una sola vez en la ruta.
public class RenglonAcreditacionPeticion
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
}

// Body de POST .../lote: { "renglones": [ ... ] }. Se validan todos los renglones
// antes de entrar al servicio; si uno falla, ASP.NET responde 400 con la clave
// del renglón (renglones[1].tipo) y no se guarda nada.
public class LoteAcreditacionPeticion
{
    [Required(ErrorMessage = "renglones es obligatorio")]
    [MinLength(1, ErrorMessage = "renglones debe traer al menos un renglón")]
    public List<RenglonAcreditacionPeticion> Renglones { get; set; } = new();
}