using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/aa_rc")]
public class AaRcController : ControllerBase
{
    private readonly IAaRcServicio _servicio;
    public AaRcController(IAaRcServicio servicio) => _servicio = servicio;

    // GET /api/aa_rc
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/aa_rc/{activAcademicasIdcurso}/{registroCalificadoCodigo}
    [HttpGet("{activAcademicasIdcurso}/{registroCalificadoCodigo}")]
    public async Task<IActionResult> ObtenerPorLlave(int activAcademicasIdcurso, int registroCalificadoCodigo)
        => Ok(await _servicio.ObtenerPorLlaveAsync(activAcademicasIdcurso, registroCalificadoCodigo));

    // POST /api/aa_rc  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearAaRcPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { activAcademicasIdcurso = fila.ActivAcademicasIdcurso, registroCalificadoCodigo = fila.RegistroCalificadoCodigo }, fila);
    }

    // PUT /api/aa_rc/{activAcademicasIdcurso}/{registroCalificadoCodigo}  -> solo los campos propios
    [HttpPut("{activAcademicasIdcurso}/{registroCalificadoCodigo}")]
    public async Task<IActionResult> Actualizar(int activAcademicasIdcurso, int registroCalificadoCodigo, [FromBody] ActualizarAaRcPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(activAcademicasIdcurso, registroCalificadoCodigo, peticion));

    // DELETE /api/aa_rc/{activAcademicasIdcurso}/{registroCalificadoCodigo}  -> retira la pareja (activo = false)
    [HttpDelete("{activAcademicasIdcurso}/{registroCalificadoCodigo}")]
    public async Task<IActionResult> Retirar(int activAcademicasIdcurso, int registroCalificadoCodigo)
    {
        await _servicio.RetirarAsync(activAcademicasIdcurso, registroCalificadoCodigo);
        return NoContent();
    }
}