namespace ApiInnovacionCurricular.Modelos;

public class AspectoNormativo
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Fuente { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
