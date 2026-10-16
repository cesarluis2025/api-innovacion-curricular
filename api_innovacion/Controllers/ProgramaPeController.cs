using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/programa_pe")]
public class ProgramaPeController : ControllerBase
{
    private readonly IProgramaPeServicio _servicio;
    public ProgramaPeController(IProgramaPeServicio servicio) => _servicio = servicio;

    // GET /api/programa_pe
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/programa_pe/{programa}/{practicaEstrategia}
    [HttpGet("{programa}/{practicaEstrategia}")]
    public async Task<IActionResult> ObtenerPorLlave(int programa, int practicaEstrategia)
        => Ok(await _servicio.ObtenerPorLlaveAsync(programa, practicaEstrategia));

    // POST /api/programa_pe  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearProgramaPePeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { programa = fila.Programa, practicaEstrategia = fila.PracticaEstrategia }, fila);
    }

    // DELETE /api/programa_pe/{programa}/{practicaEstrategia}  -> retira la pareja (activo = false)
    [HttpDelete("{programa}/{practicaEstrategia}")]
    public async Task<IActionResult> Retirar(int programa, int practicaEstrategia)
    {
        await _servicio.RetirarAsync(programa, practicaEstrategia);
        return NoContent();
    }
}