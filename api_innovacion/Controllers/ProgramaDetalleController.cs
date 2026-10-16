using Microsoft.AspNetCore.Mvc;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Servicios;

namespace ApiInnovacionCurricular.Controllers;

// Rutas del detalle de Programa. Comparte el prefijo /api/programa con ProgramaController pero
// con otra plantilla ({id}/detalle), así que no chocan. El 404 y el 409 los
// pone Middleware/ManejadorErrores.cs.
[ApiController]
[Route("api/programa")]
public class ProgramaDetalleController : ControllerBase
{
    private readonly IProgramaDetalleServicio _servicio;
    public ProgramaDetalleController(IProgramaDetalleServicio servicio) => _servicio = servicio;

    // GET /api/programa/{id}/acreditaciones
    [HttpGet("{id}/acreditaciones")]
    public async Task<IActionResult> ListarAcreditaciones(int id) => Ok(await _servicio.ListarAcreditacionesAsync(id));

    // POST /api/programa/{id}/acreditaciones/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/acreditaciones/lote")]
    public async Task<IActionResult> CrearAcreditacionesLote(int id, [FromBody] LoteAcreditacionPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearAcreditacionesLoteAsync(id, peticion));

    // GET /api/programa/{id}/registros_calificados
    [HttpGet("{id}/registros_calificados")]
    public async Task<IActionResult> ListarRegistrosCalificados(int id) => Ok(await _servicio.ListarRegistrosCalificadosAsync(id));

    // POST /api/programa/{id}/registros_calificados/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/registros_calificados/lote")]
    public async Task<IActionResult> CrearRegistrosCalificadosLote(int id, [FromBody] LoteRegistroCalificadoPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearRegistrosCalificadosLoteAsync(id, peticion));

    // GET /api/programa/{id}/actividades_academicas
    [HttpGet("{id}/actividades_academicas")]
    public async Task<IActionResult> ListarActividadesAcademicas(int id) => Ok(await _servicio.ListarActividadesAcademicasAsync(id));

    // POST /api/programa/{id}/actividades_academicas/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/actividades_academicas/lote")]
    public async Task<IActionResult> CrearActividadesAcademicasLote(int id, [FromBody] LoteActivAcademicaPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearActividadesAcademicasLoteAsync(id, peticion));

    // GET /api/programa/{id}/pasantias
    [HttpGet("{id}/pasantias")]
    public async Task<IActionResult> ListarPasantias(int id) => Ok(await _servicio.ListarPasantiasAsync(id));

    // POST /api/programa/{id}/pasantias/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/pasantias/lote")]
    public async Task<IActionResult> CrearPasantiasLote(int id, [FromBody] LotePasantiaPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearPasantiasLoteAsync(id, peticion));

    // GET /api/programa/{id}/premios
    [HttpGet("{id}/premios")]
    public async Task<IActionResult> ListarPremios(int id) => Ok(await _servicio.ListarPremiosAsync(id));

    // POST /api/programa/{id}/premios/lote  -> 201 con las filas como quedaron guardadas
    [HttpPost("{id}/premios/lote")]
    public async Task<IActionResult> CrearPremiosLote(int id, [FromBody] LotePremioPeticion peticion)
        => StatusCode(StatusCodes.Status201Created, await _servicio.CrearPremiosLoteAsync(id, peticion));
}