using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/programa_ac")]
public class ProgramaAcController : ControllerBase
{
    private readonly IProgramaAcServicio _servicio;
    public ProgramaAcController(IProgramaAcServicio servicio) => _servicio = servicio;

    // GET /api/programa_ac
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/programa_ac/{programa}/{areaConocimiento}
    [HttpGet("{programa}/{areaConocimiento}")]
    public async Task<IActionResult> ObtenerPorLlave(int programa, int areaConocimiento)
        => Ok(await _servicio.ObtenerPorLlaveAsync(programa, areaConocimiento));

    // POST /api/programa_ac  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearProgramaAcPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { programa = fila.Programa, areaConocimiento = fila.AreaConocimiento }, fila);
    }

    // DELETE /api/programa_ac/{programa}/{areaConocimiento}  -> retira la pareja (activo = false)
    [HttpDelete("{programa}/{areaConocimiento}")]
    public async Task<IActionResult> Retirar(int programa, int areaConocimiento)
    {
        await _servicio.RetirarAsync(programa, areaConocimiento);
        return NoContent();
    }
}