using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/enfoque_rc")]
public class EnfoqueRcController : ControllerBase
{
    private readonly IEnfoqueRcServicio _servicio;
    public EnfoqueRcController(IEnfoqueRcServicio servicio) => _servicio = servicio;

    // GET /api/enfoque_rc
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/enfoque_rc/{enfoque}/{registroCalificado}
    [HttpGet("{enfoque}/{registroCalificado}")]
    public async Task<IActionResult> ObtenerPorLlave(int enfoque, int registroCalificado)
        => Ok(await _servicio.ObtenerPorLlaveAsync(enfoque, registroCalificado));

    // POST /api/enfoque_rc  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearEnfoqueRcPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { enfoque = fila.Enfoque, registroCalificado = fila.RegistroCalificado }, fila);
    }

    // DELETE /api/enfoque_rc/{enfoque}/{registroCalificado}  -> retira la pareja (activo = false)
    [HttpDelete("{enfoque}/{registroCalificado}")]
    public async Task<IActionResult> Retirar(int enfoque, int registroCalificado)
    {
        await _servicio.RetirarAsync(enfoque, registroCalificado);
        return NoContent();
    }
}