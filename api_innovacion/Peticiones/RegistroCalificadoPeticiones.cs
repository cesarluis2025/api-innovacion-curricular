using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de POST /api/registro_calificado. Las anotaciones hacen que
// ASP.NET Core responda 400 solo, antes de entrar al controlador.
// programa es la llave foránea: el frontend la llena con un desplegable, nunca se digita. Si no existe o está retirada, la base responde 23503 y la API 409.
public class CrearRegistroCalificadoPeticion
{
    public int Codigo { get; set; }

    [Required(ErrorMessage = "cant_creditos es obligatorio")]
    [MaxLength(45)]
    public string CantCreditos { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_acom es obligatorio")]
    [MaxLength(45)]
    public string HoraAcom { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_ind es obligatorio")]
    [MaxLength(45)]
    public string HoraInd { get; set; } = string.Empty;

    [Required(ErrorMessage = "metodologia es obligatorio")]
    [MaxLength(45)]
    public string Metodologia { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "fecha_fin es obligatorio")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "duracion_anios es obligatorio")]
    [MaxLength(45)]
    public string DuracionAnios { get; set; } = string.Empty;

    [Required(ErrorMessage = "duracion_semestres es obligatorio")]
    [MaxLength(45)]
    public string DuracionSemestres { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo_titulacion es obligatorio")]
    [MaxLength(45)]
    public string TipoTitulacion { get; set; } = string.Empty;

    public int Programa { get; set; }
}

// Lo que llega en el body de PUT /api/registro_calificado/{codigo}. No lleva la llave
// (codigo): va en la URL y no se cambia por aquí.
public class ActualizarRegistroCalificadoPeticion
{
    [Required(ErrorMessage = "cant_creditos es obligatorio")]
    [MaxLength(45)]
    public string CantCreditos { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_acom es obligatorio")]
    [MaxLength(45)]
    public string HoraAcom { get; set; } = string.Empty;

    [Required(ErrorMessage = "hora_ind es obligatorio")]
    [MaxLength(45)]
    public string HoraInd { get; set; } = string.Empty;

    [Required(ErrorMessage = "metodologia es obligatorio")]
    [MaxLength(45)]
    public string Metodologia { get; set; } = string.Empty;

    [Required(ErrorMessage = "fecha_inicio es obligatorio")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "fecha_fin es obligatorio")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "duracion_anios es obligatorio")]
    [MaxLength(45)]
    public string DuracionAnios { get; set; } = string.Empty;

    [Required(ErrorMessage = "duracion_semestres es obligatorio")]
    [MaxLength(45)]
    public string DuracionSemestres { get; set; } = string.Empty;

    [Required(ErrorMessage = "tipo_titulacion es obligatorio")]
    [MaxLength(45)]
    public string TipoTitulacion { get; set; } = string.Empty;

    public int Programa { get; set; }
}