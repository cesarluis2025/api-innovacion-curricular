using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs a partir
// de las excepciones del servicio y de los errores 23503/23505 de la base.
[ApiController]
[Route("api/acreditacion")]
public class AcreditacionController : ControllerBase
{
    private readonly IAcreditacionServicio _servicio;
    public AcreditacionController(IAcreditacionServicio servicio) => _servicio = servicio;

    // GET /api/acreditacion
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/acreditacion/5
    [HttpGet("{resolucion}")]
    public async Task<IActionResult> ObtenerPorId(int resolucion) => Ok(await _servicio.ObtenerPorIdAsync(resolucion));

    // POST /api/acreditacion  -> 201 con la fila como quedó guardada
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAcreditacionPeticion peticion)
    {
        var fila = await _servicio.CrearAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorId), new { resolucion = fila.Resolucion }, fila);
    }

    // PUT /api/acreditacion/5
    [HttpPut("{resolucion}")]
    public async Task<IActionResult> Actualizar(int resolucion, [FromBody] ActualizarAcreditacionPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(resolucion, peticion));

    // DELETE /api/acreditacion/5  -> borrado lógico (activo = false)
    [HttpDelete("{resolucion}")]
    public async Task<IActionResult> Eliminar(int resolucion)
    {
        await _servicio.EliminarAsync(resolucion);
        return NoContent();
    }
}