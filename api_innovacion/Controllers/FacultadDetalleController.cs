using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Rutas del detalle de Facultad. Comparte el prefijo /api/facultad con FacultadController pero
// con otra plantilla ({id}/detalle), así que no chocan. El 404 y el 409 los
// pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/facultad")]
public class FacultadDetalleController : ControllerBase
{
    private readonly IFacultadDetalleServicio _servicio;
    public FacultadDetalleController(IFacultadDetalleServicio servicio) => _servicio = servicio;

    // GET /api/facultad/{id}/programas
    [HttpGet("{id}/programas")]
    public async Task<IActionResult> ListarProgramas(int id) => Ok(await _servicio.ListarProgramasAsync(id));

    // POST /api/facultad/{id}/programas/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/programas/lote")]
    public async Task<IActionResult> CrearProgramasLote(int id, [FromBody] LoteProgramaPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearProgramasLoteAsync(id, peticion));
}