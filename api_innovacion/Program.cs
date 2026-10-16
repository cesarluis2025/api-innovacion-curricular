using ApiInnovacionCurricular.Repositorios;
using ApiInnovacionCurricular.Servicios;

// Las funciones sp_ devuelven columnas en snake_case (razon_social); el modelo
// de C# usa PascalCase (RazonSocial). Con esto Dapper las une sin alias a mano.
// Debe fijarse una vez, antes de la primera consulta.
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// --- Controladores y Swagger (documentación interactiva) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Cadena de conexión: viene SOLO de la variable de entorno CONNECTION_STRING
//     (docker-compose.yml la arma con el .env). No hay valor por defecto a
//     propósito: uno con la clave escrita aquí la dejaría en el repositorio,
//     y sin ella es mejor fallar al arrancar con un mensaje claro que
//     conectarse a medias. Para correr fuera de Docker, defínela en tu terminal. ---
var cadenaConexion = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "Falta la variable de entorno CONNECTION_STRING. Copia .env.example a .env y arranca con 'docker compose up -d'.");

// --- Inyección de dependencias: cada tabla registra su repositorio y su
//     servicio. Cuando agreguemos las otras 6 tablas de esta entrega,
//     cada una suma dos líneas aquí. ---
builder.Services.AddSingleton(_ => cadenaConexion);
builder.Services.AddScoped<IAreaConocimientoRepositorio, AreaConocimientoRepositorio>();
builder.Services.AddScoped<IAreaConocimientoServicio, AreaConocimientoServicio>();
builder.Services.AddScoped<IUniversidadRepositorio, UniversidadRepositorio>();
builder.Services.AddScoped<IUniversidadServicio, UniversidadServicio>();
builder.Services.AddScoped<IAspectoNormativoRepositorio, AspectoNormativoRepositorio>();
builder.Services.AddScoped<IAspectoNormativoServicio, AspectoNormativoServicio>();
builder.Services.AddScoped<IPracticaEstrategiaRepositorio, PracticaEstrategiaRepositorio>();
builder.Services.AddScoped<IPracticaEstrategiaServicio, PracticaEstrategiaServicio>();
builder.Services.AddScoped<IEnfoqueRepositorio, EnfoqueRepositorio>();
builder.Services.AddScoped<IEnfoqueServicio, EnfoqueServicio>();
builder.Services.AddScoped<ICarInnovacionRepositorio, CarInnovacionRepositorio>();
builder.Services.AddScoped<ICarInnovacionServicio, CarInnovacionServicio>();
builder.Services.AddScoped<IAliadoRepositorio, AliadoRepositorio>();
builder.Services.AddScoped<IAliadoServicio, AliadoServicio>();

var app = builder.Build();

// Primer middleware: si algo falla más abajo, aquí se convierte en 404/409/500.
app.UseMiddleware<ApiInnovacionCurricular.Middleware.ManejadorErrores>();

// Swagger disponible siempre (no solo en desarrollo), para que el profesor
// pueda entrar directo a /swagger a probar los endpoints.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Ruta raíz simple, solo para confirmar que la API está viva.
app.MapGet("/", () => Results.Ok(new { estado = "API de innovación curricular activa" }));

app.Run();