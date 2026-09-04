using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

[ApiController]
[Route("api/enfoque")]
public class EnfoqueController : ControllerBase
{
    private readonly IEnfoqueServicio _servicio;
    public EnfoqueController(IEnfoqueServicio servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try { return Ok(await _servicio.ObtenerPorIdAsync(id)); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEnfoquePeticion peticion)
    {
        try
        {
            var entidad = await _servicio.CrearAsync(peticion);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.Id }, entidad);
        }
        catch (ConflictoExcepcion ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEnfoquePeticion peticion)
    {
        try { return Ok(await _servicio.ActualizarAsync(id, peticion)); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try { await _servicio.EliminarAsync(id); return NoContent(); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }
}
