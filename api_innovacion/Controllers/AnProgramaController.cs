using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/an_programa")]
public class AnProgramaController : ControllerBase
{
    private readonly IAnProgramaServicio _servicio;
    public AnProgramaController(IAnProgramaServicio servicio) => _servicio = servicio;

    // GET /api/an_programa
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/an_programa/{aspectoNormativo}/{programa}
    [HttpGet("{aspectoNormativo}/{programa}")]
    public async Task<IActionResult> ObtenerPorLlave(int aspectoNormativo, int programa)
        => Ok(await _servicio.ObtenerPorLlaveAsync(aspectoNormativo, programa));

    // POST /api/an_programa  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearAnProgramaPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { aspectoNormativo = fila.AspectoNormativo, programa = fila.Programa }, fila);
    }

    // DELETE /api/an_programa/{aspectoNormativo}/{programa}  -> retira la pareja (activo = false)
    [HttpDelete("{aspectoNormativo}/{programa}")]
    public async Task<IActionResult> Retirar(int aspectoNormativo, int programa)
    {
        await _servicio.RetirarAsync(aspectoNormativo, programa);
        return NoContent();
    }
}