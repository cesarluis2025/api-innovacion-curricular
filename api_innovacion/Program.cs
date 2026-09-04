using ApiInnovacionCurricular.Repositorios;
using ApiInnovacionCurricular.Servicios;

var builder = WebApplication.CreateBuilder(args);

// --- Controladores y Swagger (documentación interactiva) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Cadena de conexión: viene de la variable de entorno CONNECTION_STRING
//     (definida en docker-compose.yml). Si no existe, usa localhost para
//     cuando corres la API fuera de Docker. ---
var cadenaConexion = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? "Host=localhost;Port=15432;Database=innovacion_curricular;Username=postgres;Password=Diseno123!";

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

// Swagger disponible siempre (no solo en desarrollo), para que el profesor
// pueda entrar directo a /swagger a probar los endpoints.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Ruta raíz simple, solo para confirmar que la API está viva.
app.MapGet("/", () => Results.Ok(new { estado = "API de innovación curricular activa" }));

app.Run();
