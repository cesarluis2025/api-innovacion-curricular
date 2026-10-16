using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Body de POST /api/alianza: las dos llaves de la pareja y los campos propios.
// Las llaves se eligen en desplegables; si no existen o están retiradas la base
// responde 23503 y la API 409. Si la pareja ya existe ACTIVA, 23505 y 409; si
// existe RETIRADA, se reactiva y responde 201.
public class CrearAlianzaPeticion
{
    public int Aliado { get; set; }

    public int Departamento { get; set; }

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? Docente { get; set; }
}

// Body de PUT /api/alianza/{aliado}/{departamento}: solo los campos propios; la pareja
// va en la URL y no se cambia (para otra pareja se retira esta y se asigna otra).
public class ActualizarAlianzaPeticion
{
    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? Docente { get; set; }
}