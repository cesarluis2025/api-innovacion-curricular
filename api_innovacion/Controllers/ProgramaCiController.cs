using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/programa_ci")]
public class ProgramaCiController : ControllerBase
{
    private readonly IProgramaCiServicio _servicio;
    public ProgramaCiController(IProgramaCiServicio servicio) => _servicio = servicio;

    // GET /api/programa_ci
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/programa_ci/{programa}/{carInnovacion}
    [HttpGet("{programa}/{carInnovacion}")]
    public async Task<IActionResult> ObtenerPorLlave(int programa, int carInnovacion)
        => Ok(await _servicio.ObtenerPorLlaveAsync(programa, carInnovacion));

    // POST /api/programa_ci  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearProgramaCiPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { programa = fila.Programa, carInnovacion = fila.CarInnovacion }, fila);
    }

    // DELETE /api/programa_ci/{programa}/{carInnovacion}  -> retira la pareja (activo = false)
    [HttpDelete("{programa}/{carInnovacion}")]
    public async Task<IActionResult> Retirar(int programa, int carInnovacion)
    {
        await _servicio.RetirarAsync(programa, carInnovacion);
        return NoContent();
    }
}