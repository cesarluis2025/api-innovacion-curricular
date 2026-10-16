using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Sin try/catch: el 404 y el 409 los pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/docente_departamento")]
public class DocenteDepartamentoController : ControllerBase
{
    private readonly IDocenteDepartamentoServicio _servicio;
    public DocenteDepartamentoController(IDocenteDepartamentoServicio servicio) => _servicio = servicio;

    // GET /api/docente_departamento
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _servicio.ListarAsync());

    // GET /api/docente_departamento/{docente}/{departamento}
    [HttpGet("{docente}/{departamento}")]
    public async Task<IActionResult> ObtenerPorLlave(int docente, int departamento)
        => Ok(await _servicio.ObtenerPorLlaveAsync(docente, departamento));

    // POST /api/docente_departamento  -> 201 (también cuando reactiva una pareja retirada)
    [HttpPost]
    public async Task<IActionResult> Asignar([FromBody] CrearDocenteDepartamentoPeticion peticion)
    {
        var fila = await _servicio.AsignarAsync(peticion);
        return CreatedAtAction(nameof(ObtenerPorLlave), new { docente = fila.Docente, departamento = fila.Departamento }, fila);
    }

    // PUT /api/docente_departamento/{docente}/{departamento}  -> solo los campos propios
    [HttpPut("{docente}/{departamento}")]
    public async Task<IActionResult> Actualizar(int docente, int departamento, [FromBody] ActualizarDocenteDepartamentoPeticion peticion)
        => Ok(await _servicio.ActualizarAsync(docente, departamento, peticion));

    // DELETE /api/docente_departamento/{docente}/{departamento}  -> retira la pareja (activo = false)
    [HttpDelete("{docente}/{departamento}")]
    public async Task<IActionResult> Retirar(int docente, int departamento)
    {
        await _servicio.RetirarAsync(docente, departamento);
        return NoContent();
    }
}