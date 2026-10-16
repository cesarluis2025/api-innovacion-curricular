using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Rutas del detalle de Universidad. Comparte el prefijo /api/universidad con UniversidadController pero
// con otra plantilla ({id}/detalle), así que no chocan. El 404 y el 409 los
// pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/universidad")]
public class UniversidadDetalleController : ControllerBase
{
    private readonly IUniversidadDetalleServicio _servicio;
    public UniversidadDetalleController(IUniversidadDetalleServicio servicio) => _servicio = servicio;

    // GET /api/universidad/{id}/facultades
    [HttpGet("{id}/facultades")]
    public async Task<IActionResult> ListarFacultades(int id) => Ok(await _servicio.ListarFacultadesAsync(id));

    // POST /api/universidad/{id}/facultades/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/facultades/lote")]
    public async Task<IActionResult> CrearFacultadesLote(int id, [FromBody] LoteFacultadPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearFacultadesLoteAsync(id, peticion));
}