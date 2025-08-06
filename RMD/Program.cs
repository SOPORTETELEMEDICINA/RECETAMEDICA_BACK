
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RMD.Data;
using RMD.Interface.Auth;
using RMD.Interface.CargaCatalogos;
using RMD.Interface.Catalogo;
using RMD.Interface.Consulta;
using RMD.Interface.Dashboard;
using RMD.Interface.Medicos;
using RMD.Interface.Pacientes;
using RMD.Interface.PuntoVenta;
using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Interface.Sucursales;
using RMD.Interface.Tutores;
using RMD.Interface.Usuarios;
using RMD.Interface.Vidal;
using RMD.Service.Auth;
using RMD.Service.CargaCatalogos;
using RMD.Service.Catalogo;
using RMD.Service.Consulta;
using RMD.Service.Dashboard;
using RMD.Service.Medicos;
using RMD.Service.Pacientes;
using RMD.Service.PuntoVenta;
using RMD.Service.Receta;
using RMD.Service.ServiciosInternos;
using RMD.Service.Sucursales;
using RMD.Service.Tutores;
using RMD.Service.Usuarios;
using RMD.Service.Vidal.Allergy;
using RMD.Service.Vidal.CIM10;
using RMD.Service.Vidal.Molecule;
using RMD.Service.Vidal.Tools;
using RMD.Shared.Models.Login;
using RMD.Shared.Utils.Interface;
using RMD.Shared.Utils.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de cadenas y DbContexts
string environment = builder.Configuration["Environment"];

// Obtiene la cadena encriptada del appsettings
var encryptedConnStr = builder.Configuration.GetConnectionString(environment);

if (string.IsNullOrWhiteSpace(encryptedConnStr))
    throw new Exception($"No se encontró una cadena de conexión encriptada para el entorno '{environment}'.");

// Rutas posibles
var basePaths = new[]
{
    @"C:\Secrets\RecetaMedica",
    @"D:\Secrets\RecetaMedica"
};

string? keyFile = basePaths
    .Select(path => Path.Combine(path, $"{environment.ToLower()}.key"))
    .FirstOrDefault(File.Exists);


if (keyFile is null)
    throw new Exception("No se encontró el archivo de clave de encriptación para el entorno actual.");

// Desencripta la cadena de conexión
var key = File.ReadAllText(keyFile).Trim();
var connectionString = EncryptionHelper.Decrypt(encryptedConnStr, Convert.FromBase64String(key));
// Leer clave para cifrado de respuestas (ResponseFromService)

/*****************************************************************/
var commonKeyPath = basePaths
    .Select(path => Path.Combine(path, "common.key"))
    .FirstOrDefault(File.Exists);

if (commonKeyPath is null)
    throw new Exception("No se encontró el archivo common.key.");

var commonKey = Convert.FromBase64String(File.ReadAllText(commonKeyPath).Trim());
builder.Services.AddSingleton(commonKey);
// JWT Authentication
// 🔐 Leer archivo .jwt
var jwtKeyFile = basePaths
    .Select(path => Path.Combine(path, $"{environment.ToLower()}.jwt"))
    .FirstOrDefault(File.Exists);

if (jwtKeyFile is null)
    throw new Exception("No se encontró el archivo de clave JWT para el entorno actual.");

var responseKeyFile = basePaths
    .Select(path => Path.Combine(path, "response.key"))
    .FirstOrDefault(File.Exists);

if (responseKeyFile is null)
    throw new Exception("No se encontró el archivo de clave para cifrado de respuestas (response.key).");

var jwtKey = File.ReadAllText(jwtKeyFile).Trim();
var responseKey = File.ReadAllText(responseKeyFile).Trim();
var responseEncryptionKey = Convert.FromBase64String(responseKey);

builder.Services.AddSingleton(new JwtKeyHolder(jwtKey, responseKey));


//builder.Services.AddSingleton<string>(connectionString); // <-- explícito, sin warning
//builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<CatalogoWebServiceDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<UsuariosDBContext>(o =>
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<MedicosDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<PacientesDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<RecetasDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<SucursalesDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<DashboardDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<VidalAPIDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<ConsultaDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<PuntoVentaDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<VidalDBContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddDbContext<CatalogoDbContext>(o => 
    o.UseSqlServer(connectionString));
builder.Services.AddMemoryCache();

// Registro de servicios de dominio
//builder.Services.AddScoped<IConfiguracionGlobalService, ConfiguracionGlobalService>();

builder.Services.AddScoped<ICargaCatalogosService, CargaCatalogosService>();
/* * Aquí se registran los servicios de dominio.
 * Asegúrate de que cada servicio implementa su respectiva interfaz.
 */
/* * Catalogos: **/
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<ICatAsentamientoCiudadService, CatAsentamientoCiudadService>();
builder.Services.AddScoped<ICatAsentamientosService, CatAsentamientosService>();
builder.Services.AddScoped<ICatCiudadesService, CatCiudadesService>();
builder.Services.AddScoped<ICatCpService, CatCpService>();
builder.Services.AddScoped<ICatEntidadesFederativasService, CatEntidadesFederativasService>();
builder.Services.AddScoped<ICatEventosDeSaludService, CatEventosDeSaludService>();
builder.Services.AddScoped<ICatMunicipiosService, CatMunicipiosService>();
builder.Services.AddScoped<ICatTipoAsentamientoService, CatTipoAsentamientoService>();

/* * Pacientes: **/
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IHelperPacienteServiceES, HelperPacienteServiceES>();
builder.Services.AddScoped<IEventosSaludService, EventosSaludService>();
/* * Recetas: **/
builder.Services.AddScoped<IDetalleRecetaService, DetalleRecetaService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<IHelperRecetaService, HelperRecetaService>();
builder.Services.AddScoped<IHelperRecetaServiceES, HelperRecetaServiceES>();
builder.Services.AddScoped<IHelperRecetaServiceMX, HelperRecetaServiceMX>();
builder.Services.AddScoped<IAlertaTomaService, AlertaTomaService>();
builder.Services.AddScoped<IRecetaCatalogosService, RecetaCatalogosService>();

/*  Tutores */
builder.Services.AddScoped<ITutorService, TutorService>();

builder.Services.AddScoped<ICatGrupoEmpresarialService, CatGrupoEmpresarialService>();
builder.Services.AddScoped<ITipoUsuarioService, TipoUsuarioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();



builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IToolsService, ToolsService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ICargaCatalogosWebService, CargaCatalogosWebService>();
// Aquí agrega los HttpClients personalizados
builder.Services.AddHttpClient("VidalClientMX", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VidalApi:BaseUrl"]);
});

builder.Services.AddHttpClient("VidalClientES", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VidalApiES:BaseUrl"]);
});
builder.Services.AddScoped<ICargaCatalogosWebService, CargaCatalogosWebService>();
builder.Services.AddHttpClient<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IPuntoVentaService, PuntoVentaService>();
builder.Services.AddScoped<ICatalogoNotificacionService, CatalogoNotificacionServices>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDapperService>(_ =>
    new DapperService(connectionString));
/**************************************************************************/
/***************************    VIDAL   ***********************************/
/**************************************************************************/
builder.Services.AddScoped<ICIM10Service, CIM10Service>();
builder.Services.AddScoped<IAllergyService, AllergyService>();
builder.Services.AddScoped<IMoleculeService, MoleculeService>();
// Filtros y cifrado de respuestas
builder.Services.AddScoped<IEncryptionService>(_ =>
    new AesEncryptionService(responseEncryptionKey));

builder.Services.AddScoped<EncryptResponseFilter>();
builder.Services.AddScoped<ValidateTokenFilter>();
builder.Services.AddScoped<CifradoHelper>();
builder.Services.AddScoped<FolioHelper>();
builder.Services.AddScoped<CronRecetaService>(); // ← Necesaria para la inyección



// Hangfire
builder.Services.AddScoped<TokenCleanupJob>();
builder.Services.AddHangfire(cfg =>
{
    cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
       {
           CommandBatchMaxTimeout = TimeSpan.FromMinutes(60),
           SlidingInvisibilityTimeout = TimeSpan.FromMinutes(60),
           QueuePollInterval = TimeSpan.FromSeconds(60),
           UseRecommendedIsolationLevel = true,
           DisableGlobalLocks = true
       });
});
builder.Services.AddHangfireServer();

// MVC & Swagger
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<EncryptResponseFilter>();
})
// Configura el serializer de MVC para camelCase
.AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = $"RMD_{environment} API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer {token_encriptado}'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }
        ] = Array.Empty<string>()
    });
});

// CORS
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", pb =>
    pb.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


// 🔐 Leer configuración común desde appsettings.json
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKeyHolder = builder.Services
        .Where(sd => sd.ServiceType == typeof(JwtKeyHolder))
        .Select(sd => (JwtKeyHolder?)sd.ImplementationInstance)
        .FirstOrDefault();

    if (jwtKeyHolder == null)
        throw new Exception("JwtKeyHolder no está registrado correctamente.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyHolder.Key),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"RMD_{environment} API v1");
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

// 1) Desencripta JWT cifrado en header
app.UseMiddleware<DecryptJwtMiddleware>();

// 2) (Opcional) Renueva token
app.UseMiddleware<RenewTokenMiddleware>();

// 3) Manejo de errores y logging
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

// 4) Autenticación / Autorización con el JWT ya desencriptado
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

// Hangfire Dashboard + Jobs
app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<TokenCleanupJob>(
    "TokenCleanupJob", job => job.CleanupAsync(), "0 */2 * * *");
//RecurringJob.AddOrUpdate<ICargaCatalogosWebService>(
//    "CargaCatalogosWebServiceJob",
//    svc => svc.CargarCatalogosWebService(),
//    Cron.Daily);
RecurringJob.AddOrUpdate<ICargaCatalogosWebService>(
    "CargaCatalogosWebServiceJob",
    svc => svc.CargarCatalogosWebService(),
    "0 0 2,12,22 * *" // a las 00:00 los días 2, 12 y 22
);
// Días que terminan en 1: 1, 11, 21, 31
RecurringJob.AddOrUpdate<ICargaCatalogosService>(
    "CargaCatalogosApiVidalJob",
    svc => svc.ReloadCatalogs(),
    "0 0 1,11,21,31 * *" // a las 00:00 los días 1, 11, 21 y 31
);
RecurringJob.AddOrUpdate<CronRecetaService>(
    "CronRecetaServiceJob",
    svc => svc.ObtenerDetalleCronicoESAsync(),
    "0 2 * * *"
);
RecurringJob.AddOrUpdate<IToolsService>(
    "GuardarAlertasDesdeVidalJob",
    svc => svc.GuardarTodasLasAlertasDesdeVidalAsync(),
    "0 0 * * *" // todos los días a la medianoche
);
app.Run();
