using System.ComponentModel.DataAnnotations;

namespace ApiInnovacionCurricular.Peticiones;

// Body de POST /api/programa_ci: las dos llaves de la pareja.
// Las llaves se eligen en desplegables; si no existen o están retiradas la base
// responde 23503 y la API 409. Si la pareja ya existe ACTIVA, 23505 y 409; si
// existe RETIRADA, se reactiva y responde 201.
public class CrearProgramaCiPeticion
{
    public int Programa { get; set; }

    public int CarInnovacion { get; set; }
}