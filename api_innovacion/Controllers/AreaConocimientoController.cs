using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

[ApiController]
[Route("api/area_conocimiento")]
public class AreaConocimientoController : ControllerBase
{
    private readonly IAreaConocimientoServicio _servicio;

    public AreaConocimientoController(IAreaConocimientoServicio servicio)
    {
        _servicio = servicio;
    }

    // GET /api/area_conocimiento
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var areas = await _servicio.ListarAsync();
        return Ok(areas); // 200
    }

    // GET /api/area_conocimiento/5
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var area = await _servicio.ObtenerPorIdAsync(id);
            return Ok(area); // 200
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message }); // 404
        }
    }

    // POST /api/area_conocimiento
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAreaConocimientoPeticion peticion)
    {
        // Si algún [Required] de la petición falló, ASP.NET Core ya
        // respondió 400 automáticamente y este código ni se ejecuta.
        try
        {
            var area = await _servicio.CrearAsync(peticion);
            // 201 Created, con la ubicación del nuevo recurso en el header Location.
            return CreatedAtAction(nameof(ObtenerPorId), new { id = area.Id }, area);
        }
        catch (ConflictoExcepcion ex)
        {
            return BadRequest(new { mensaje = ex.Message }); // 400
        }
    }

    // PUT /api/area_conocimiento/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAreaConocimientoPeticion peticion)
    {
        try
        {
            var area = await _servicio.ActualizarAsync(id, peticion);
            return Ok(area); // 200
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message }); // 404
        }
    }

    // DELETE /api/area_conocimiento/5  → borrado lógico
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _servicio.EliminarAsync(id);
            return NoContent(); // 204: se eliminó (lógicamente), sin contenido que devolver
        }
        catch (NoEncontradoExcepcion ex)
        {
            return NotFound(new { mensaje = ex.Message }); // 404
        }
    }
}
