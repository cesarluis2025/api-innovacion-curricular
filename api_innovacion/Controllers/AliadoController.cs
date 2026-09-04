using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

[ApiController]
[Route("api/aliado")]
public class AliadoController : ControllerBase
{
    private readonly IAliadoServicio _servicio;
    public AliadoController(IAliadoServicio servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // La ruta usa {nit}, no {id}, porque la llave primaria de esta tabla es nit.
    [HttpGet("{nit}")]
    public async Task<IActionResult> ObtenerPorNit(int nit)
    {
        try { return Ok(await _servicio.ObtenerPorNitAsync(nit)); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAliadoPeticion peticion)
    {
        try
        {
            var aliado = await _servicio.CrearAsync(peticion);
            return CreatedAtAction(nameof(ObtenerPorNit), new { nit = aliado.Nit }, aliado);
        }
        catch (ConflictoExcepcion ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("{nit}")]
    public async Task<IActionResult> Actualizar(int nit, [FromBody] ActualizarAliadoPeticion peticion)
    {
        try { return Ok(await _servicio.ActualizarAsync(nit, peticion)); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{nit}")]
    public async Task<IActionResult> Eliminar(int nit)
    {
        try { await _servicio.EliminarAsync(nit); return NoContent(); }
        catch (NoEncontradoExcepcion ex) { return NotFound(new { mensaje = ex.Message }); }
    }
}
