using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/alianza")]
public class AlianzaController : ControllerBase
{
    private readonly IAlianzaServicio _servicio;
    public AlianzaController(IAlianzaServicio servicio) => _servicio = servicio;

    // GET /api/alianza
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/alianza/{aliado}/{departamento}
    [HttpGet("{aliado}/{departamento}")]
    public async Task<IActionResult> ObtenerPorLlave(int aliado, int departamento)
        => Ok(await _servicio.ObtenerPorLlaveAsync(aliado, departamento));

    // POST /api/alianza  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearAlianzaPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { aliado = fila.Aliado, departamento = fila.Departamento }, fila);
    }

    // PUT /api/alianza/{aliado}/{departamento}  -> solo los campos propios
    [HttpPut("{aliado}/{departamento}")]
    public async Task<IActionResult> Actualizar(int aliado, int departamento, [FromBody] ActualizarAlianzaPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(aliado, departamento, peticion));

    // DELETE /api/alianza/{aliado}/{departamento}  -> retira la pareja (activo = false)
    [HttpDelete("{aliado}/{departamento}")]
    public async Task<IActionResult> Retirar(int aliado, int departamento)
    {
        await _servicio.RetirarAsync(aliado, departamento);
        return NoContent();
    }
}