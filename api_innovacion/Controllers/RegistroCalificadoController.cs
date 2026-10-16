using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs a partir
// de las excepciones del servicio y de los errores 23503/23505 de la base.
[ApiController]
[Route("api/registro_calificado")]
public class RegistroCalificadoController : ControllerBase
{
    private readonly IRegistroCalificadoServicio _servicio;
    public RegistroCalificadoController(IRegistroCalificadoServicio servicio) => _servicio = servicio;

    // GET /api/registro_calificado
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/registro_calificado/5
    [HttpGet("{codigo}")]
    public async Task<IActionResult> ObtenerPorId(int codigo) => Ok(await _servicio.ObtenerPorIdAsync(codigo));

    // POST /api/registro_calificado  -> 201 con la fila como quedó guardada
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearRegistroCalificadoPeticion peticion)
    {
        var fila = await _servicio.CrearAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorId), new { codigo = fila.Codigo }, fila);
    }

    // PUT /api/registro_calificado/5
    [HttpPut("{codigo}")]
    public async Task<IActionResult> Actualizar(int codigo, [FromBody] ActualizarRegistroCalificadoPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(codigo, peticion));

    // DELETE /api/registro_calificado/5  -> borrado lógico (activo = false)
    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Eliminar(int codigo)
    {
        await _servicio.EliminarAsync(codigo);
        return NoContent();
    }
}