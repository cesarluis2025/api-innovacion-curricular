using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Body de POST /api/docente_departamento: las dos llaves de la pareja y los campos propios.
// Las llaves se eligen en desplegables; si no existen o están retiradas la base
// responde 23503 y la API 409. Si la pareja ya existe ACTIVA, 23505 y 409; si
// existe RETIRADA, se reactiva y responde 201.
// docente NO es llave foránea: su tabla es de otro módulo, no se valida contra nada.
public class CrearDocenteDepartamentoPeticion
{
    public int Docente { get; set; }

    public int Departamento { get; set; }

    [Required(ErrorMessage = "dedicacion es obligatorio")]
    [MaxLength(15)]
    public string Dedicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "modalidad es obligatorio")]
    [MaxLength(45)]
    public string Modalidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_ingreso es obligatorio")]
    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }
}

// Body de PUT /api/docente_departamento/{docente}/{departamento}: solo los campos propios; la pareja
// va en la URL y no se cambia (para otra pareja se retira esta y se asigna otra).
public class ActualizarDocenteDepartamentoPeticion
{
    [Required(ErrorMessage = "dedicacion es obligatorio")]
    [MaxLength(15)]
    public string Dedicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "modalidad es obligatorio")]
    [MaxLength(45)]
    public string Modalidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_ingreso es obligatorio")]
    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }
}