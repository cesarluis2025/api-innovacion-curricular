using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs a partir
// de las excepciones del servicio y de los errores 23503/23505 de la base.
[ApiController]
[Route("api/programa")]
public class ProgramaController : ControllerBase
{
    private readonly IProgramaServicio _servicio;
    public ProgramaController(IProgramaServicio servicio) => _servicio = servicio;

    // GET /api/programa
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/programa/5
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id) => Ok(await _servicio.ObtenerPorIdAsync(id));

    // POST /api/programa  -> 201 con la fila como quedó guardada
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProgramaPeticion peticion)
    {
        var fila = await _servicio.CrearAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = fila.Id }, fila);
    }

    // PUT /api/programa/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProgramaPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(id, peticion));

    // DELETE /api/programa/5  -> borrado lógico (activo = false)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}