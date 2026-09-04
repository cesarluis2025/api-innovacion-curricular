using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Lo que llega en el body de un POST /api/area_conocimiento.
// Las anotaciones ([Required], [MaxLength]) hacen que ASP.NET Core valide
// solo, y si algo falla, la API responde 400 automáticamente antes de
// que el código del controlador se ejecute.
public class CrearAreaConocimientoPeticion
{
    [Required(ErrorMessage = "el id es obligatorio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "gran_area es obligatorio")]
    [MaxLength(60)]
    public string GranArea { get; set; } = string.Empty;

    [Required(ErrorMessage = "area es obligatorio")]
    [MaxLength(60)]
    public string Area { get; set; } = string.Empty;

    [Required(ErrorMessage = "disciplina es obligatorio")]
    [MaxLength(60)]
    public string Disciplina { get; set; } = string.Empty;
}

// Lo que llega en el body de un PUT /api/area_conocimiento/{id}.
// No incluye el Id: ese va en la url, no se cambia por aquí.
public class ActualizarAreaConocimientoPeticion
{
    [Required(ErrorMessage = "gran_area es obligatorio")]
    [MaxLength(60)]
    public string GranArea { get; set; } = string.Empty;

    [Required(ErrorMessage = "area es obligatorio")]
    [MaxLength(60)]
    public string Area { get; set; } = string.Empty;

    [Required(ErrorMessage = "disciplina es obligatorio")]
    [MaxLength(60)]
    public string Disciplina { get; set; } = string.Empty;
}
