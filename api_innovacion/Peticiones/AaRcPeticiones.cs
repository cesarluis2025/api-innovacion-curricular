using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Body de POST /api/aa_rc: las dos llaves de la pareja y los campos propios.
// Las llaves se eligen en desplegables; si no existen o están retiradas la base
// responde 23503 y la API 409. Si la pareja ya existe ACTIVA, 23505 y 409; si
// existe RETIRADA, se reactiva y responde 201.
public class CrearAaRcPeticion
{
    public int ActivAcademicasIdcurso { get; set; }

    public int RegistroCalificadoCodigo { get; set; }

    [Required(ErrorMessage = "componente es obligatorio")]
    [MaxLength(45)]
    public string Componente { get; set; } = string.Empty;

    [Required(ErrorMessage = "semestre es obligatorio")]
    [MaxLength(45)]
    public string Semestre { get; set; } = string.Empty;
}

// Body de PUT /api/aa_rc/{activAcademicasIdcurso}/{registroCalificadoCodigo}: solo los campos propios; la pareja
// va en la URL y no se cambia (para otra pareja se retira esta y se asigna otra).
public class ActualizarAaRcPeticion
{
    [Required(ErrorMessage = "componente es obligatorio")]
    [MaxLength(45)]
    public string Componente { get; set; } = string.Empty;

    [Required(ErrorMessage = "semestre es obligatorio")]
    [MaxLength(45)]
    public string Semestre { get; set; } = string.Empty;
}