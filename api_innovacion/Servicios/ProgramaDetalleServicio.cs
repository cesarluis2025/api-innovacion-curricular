using ApiInnovacionCurricular.Excepciones;
using ApiInnovacionCurricular.Modelos;
using ApiInnovacionCurricular.Peticiones;
using ApiInnovacionCurricular.Repositorios;

namespace ApiInnovacionCurricular.Servicios;

// Maestro-detalle de Programa: el detalle no existe sin su maestro, así que se lee y se
// agrega DESDE el maestro (6_contracts.md). Primero se comprueba que el maestro exista,
// para responder 404 antes de tocar el detalle.
public interface IProgramaDetalleServicio
{
    Task<IEnumerable<Acreditacion>> ListarAcreditacionesAsync(int id);
    Task<IEnumerable<Acreditacion>> CrearAcreditacionesLoteAsync(int id, LoteAcreditacionPeticion peticion);
    Task<IEnumerable<RegistroCalificado>> ListarRegistrosCalificadosAsync(int id);
    Task<IEnumerable<RegistroCalificado>> CrearRegistrosCalificadosLoteAsync(int id, LoteRegistroCalificadoPeticion peticion);
    Task<IEnumerable<ActivAcademica>> ListarActividadesAcademicasAsync(int id);
    Task<IEnumerable<ActivAcademica>> CrearActividadesAcademicasLoteAsync(int id, LoteActivAcademicaPeticion peticion);
    Task<IEnumerable<Pasantia>> ListarPasantiasAsync(int id);
    Task<IEnumerable<Pasantia>> CrearPasantiasLoteAsync(int id, LotePasantiaPeticion peticion);
    Task<IEnumerable<Premio>> ListarPremiosAsync(int id);
    Task<IEnumerable<Premio>> CrearPremiosLoteAsync(int id, LotePremioPeticion peticion);
}

public class ProgramaDetalleServicio : IProgramaDetalleServicio
{
    private readonly IProgramaRepositorio _maestros;
    private readonly IAcreditacionRepositorio _acreditacion;
    private readonly IRegistroCalificadoRepositorio _registroCalificado;
    private readonly IActivAcademicaRepositorio _activAcademica;
    private readonly IPasantiaRepositorio _pasantia;
    private readonly IPremioRepositorio _premio;

    public ProgramaDetalleServicio(IProgramaRepositorio maestros, IAcreditacionRepositorio acreditacion, IRegistroCalificadoRepositorio registroCalificado, IActivAcademicaRepositorio activAcademica, IPasantiaRepositorio pasantia, IPremioRepositorio premio)
    {
        _maestros = maestros;
        _acreditacion = acreditacion;
        _registroCalificado = registroCalificado;
        _activAcademica = activAcademica;
        _pasantia = pasantia;
        _premio = premio;
    }

    private async Task ExigirMaestroAsync(int id)
    {
        if (await _maestros.ObtenerPorIdAsync(id) is null)
            throw new NoEncontradoExcepcion($"no existe un programa con id {id}");
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<Acreditacion>> ListarAcreditacionesAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _acreditacion.ListarAsync()).Where(x => x.Programa == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_acreditacion_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<Acreditacion>> CrearAcreditacionesLoteAsync(int id, LoteAcreditacionPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new Acreditacion
        {
            Resolucion = r.Resolucion,
            Tipo = r.Tipo,
            Calificacion = r.Calificacion,
            FechaInicio = r.FechaInicio,
            FechaFin = r.FechaFin,
        }).ToList();
        return await _acreditacion.CrearLoteAsync(id, filas);
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<RegistroCalificado>> ListarRegistrosCalificadosAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _registroCalificado.ListarAsync()).Where(x => x.Programa == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_registro_calificado_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<RegistroCalificado>> CrearRegistrosCalificadosLoteAsync(int id, LoteRegistroCalificadoPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new RegistroCalificado
        {
            Codigo = r.Codigo,
            CantCreditos = r.CantCreditos,
            HoraAcom = r.HoraAcom,
            HoraInd = r.HoraInd,
            Metodologia = r.Metodologia,
            FechaInicio = r.FechaInicio,
            FechaFin = r.FechaFin,
            DuracionAnios = r.DuracionAnios,
            DuracionSemestres = r.DuracionSemestres,
            TipoTitulacion = r.TipoTitulacion,
        }).ToList();
        return await _registroCalificado.CrearLoteAsync(id, filas);
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<ActivAcademica>> ListarActividadesAcademicasAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _activAcademica.ListarAsync()).Where(x => x.Disenio == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_activ_academica_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<ActivAcademica>> CrearActividadesAcademicasLoteAsync(int id, LoteActivAcademicaPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new ActivAcademica
        {
            Id = r.Id,
            Nombre = r.Nombre,
            NumCreditos = r.NumCreditos,
            Tipo = r.Tipo,
            AreaFormacion = r.AreaFormacion,
            HAcom = r.HAcom,
            HIndep = r.HIndep,
            Idioma = r.Idioma,
            Espejo = r.Espejo,
            EntidadEspejo = ActivAcademicaServicio.TextoEspejo(r.Espejo, r.EntidadEspejo),
            PaisEspejo = ActivAcademicaServicio.TextoEspejo(r.Espejo, r.PaisEspejo),
        }).ToList();
        return await _activAcademica.CrearLoteAsync(id, filas);
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<Pasantia>> ListarPasantiasAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _pasantia.ListarAsync()).Where(x => x.Programa == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_pasantia_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<Pasantia>> CrearPasantiasLoteAsync(int id, LotePasantiaPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new Pasantia
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Pais = r.Pais,
            Empresa = r.Empresa,
            Descripcion = r.Descripcion,
        }).ToList();
        return await _pasantia.CrearLoteAsync(id, filas);
    }

    // Detalle ACTIVO del maestro. Se filtra aquí sobre la lista activa: el volumen es
    // pequeño y evita una función sp_ más por cada detalle.
    public async Task<IEnumerable<Premio>> ListarPremiosAsync(int id)
    {
        await ExigirMaestroAsync(id);
        return (await _premio.ListarAsync()).Where(x => x.Programa == id).ToList();
    }

    // Todos los renglones en UNA transacción (función sp_crear_premio_lote): si uno
    // choca, la base no guarda ninguno y el manejador de errores responde 409.
    public async Task<IEnumerable<Premio>> CrearPremiosLoteAsync(int id, LotePremioPeticion peticion)
    {
        await ExigirMaestroAsync(id);
        var filas = peticion.Renglones.Select(r => new Premio
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion,
            Fecha = r.Fecha,
            EntidadOtorgante = r.EntidadOtorgante,
            Pais = r.Pais,
        }).ToList();
        return await _premio.CrearLoteAsync(id, filas);
    }
}