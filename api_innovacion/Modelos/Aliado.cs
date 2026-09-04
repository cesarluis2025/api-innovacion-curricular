namespace ApiInnovacionCurricular.Modelos;

public class Aliado
{
    public int Nit { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreContacto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
