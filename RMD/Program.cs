using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Auth;
using RMD.Interface.Catalogos;
using RMD.Interface.Consulta;
using RMD.Interface.Dashborad;
using RMD.Interface.Medicos;
using RMD.Interface.Pacientes;
using RMD.Interface.Recetas;
using RMD.Interface.Sucursales;
using RMD.Interface.Usuarios;
using RMD.Interface.Vidal;
using RMD.Middleware;
using RMD.Service.Auth;
using RMD.Service.Catalogos;
using RMD.Service.Consulta;
using RMD.Service.Dashboard;
using RMD.Service.Medicos;
using RMD.Service.Pacientes;
using RMD.Service.Recetas;
using RMD.Service.Sucursales;
using RMD.Service.Usuarios;
using RMD.Service.Vidal.ByAllergy;
using RMD.Service.Vidal.ByATC;
using RMD.Service.Vidal.ByCIM10;
using RMD.Service.Vidal.ByForeignProduct;
using RMD.Service.Vidal.ByIndication;
using RMD.Service.Vidal.ByIndicationGroup;
using RMD.Service.Vidal.ByMolecule;
using RMD.Service.Vidal.ByPackage;
using RMD.Service.Vidal.ByProduct;
using RMD.Service.Vidal.ByRoute;
using RMD.Service.Vidal.BySideEffect;
using RMD.Service.Vidal.ByUCD;
using RMD.Service.Vidal.ByUCDV;
using RMD.Service.Vidal.ByUnit;
using RMD.Service.Vidal.ByVMP;
using RMD.Service.Vidal.ByVTM;
using RMD.Service.Vidal.CargaCatalogos;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Obtener el entorno desde el archivo de configuración
string environment = builder.Configuration["Environment"];

// Seleccionar la cadena de conexión según el entorno
string connectionString = builder.Configuration.GetConnectionString(environment);

// Configuración de DbContexts con la cadena de conexión seleccionada
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<UsuariosDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<MedicosDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<PacientesDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<RecetasDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<SucursalesDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<DashboardDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<VidalDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registro de servicios
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<ICatGrupoEmpresarialService, CatGrupoEmpresarialService>();
builder.Services.AddScoped<ITipoUsuarioService, TipoUsuarioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<IDetalleRecetaService, DetalleRecetaService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();

// Registro de IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<ICargaCatalogosService, CargaCatalogosService>();
builder.Services.AddScoped<ICargaCatalogosService, CargaCatalogosService>();

// Registro de servicios para Vidal API
builder.Services.AddHttpClient<IAllergyService, AllergyService>();
builder.Services.AddScoped<IAllergyService, AllergyService>();

builder.Services.AddHttpClient<IATCService, ATCService>();
builder.Services.AddScoped<IATCService, ATCService>();

builder.Services.AddHttpClient<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

builder.Services.AddHttpClient<IForeignProductService, ForeignProductService>();
builder.Services.AddScoped<IForeignProductService, ForeignProductService>();

builder.Services.AddHttpClient<IIndicationService, IndicationService>();
builder.Services.AddScoped<IIndicationService, IndicationService>();

builder.Services.AddHttpClient<IIndicationGroupService, IndicationGroupService>();
builder.Services.AddScoped<IIndicationGroupService, IndicationGroupService>();

builder.Services.AddHttpClient<IMoleculeService, MoleculeService>();
builder.Services.AddScoped<IMoleculeService, MoleculeService>();

builder.Services.AddHttpClient<IPackageService, PackageService>();
builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddHttpClient<ICIM10Service, CIM10Service>();
builder.Services.AddScoped<ICIM10Service, CIM10Service>();

builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpClient<IRouteService, RouteService>();
builder.Services.AddScoped<IRouteService, RouteService>();

builder.Services.AddHttpClient<ISideEffectService, SideEffectService>();
builder.Services.AddScoped<ISideEffectService, SideEffectService>();

builder.Services.AddHttpClient<IUCDService, UCDService>();
builder.Services.AddScoped<IUCDService, UCDService>();

builder.Services.AddHttpClient<IUcdvService, UcdvService>();
builder.Services.AddScoped<IUcdvService, UcdvService>();

builder.Services.AddHttpClient<IUnitService, UnitService>();
builder.Services.AddScoped<IUnitService, UnitService>();

builder.Services.AddHttpClient<IVMPService, VMPService>();
builder.Services.AddScoped<IVMPService, VMPService>();

builder.Services.AddHttpClient<IVTMService, VTMService>();
builder.Services.AddScoped<IVTMService, VTMService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<CifradoHelper>();


builder.Services.AddScoped<ValidateTokenFilter>();
// Configuración de controladores
builder.Services.AddControllers();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var apiTitle = $"RMD_{environment} API";  // Cambia el título dinámicamente según el entorno
    c.SwaggerDoc("v1", new OpenApiInfo { Title = apiTitle, Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' [espacio] y luego su token en el campo de texto a continuación.\n\nEjemplo: \"Bearer abc123\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
    {
        throw new InvalidOperationException("JWT key is not configured correctly. It must be at least 32 characters long.");
    }

    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    {
        throw new InvalidOperationException("JWT Issuer or Audience is not set in the configuration.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Añade lógica adicional si es necesario
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Log o manejo del error de autenticación
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"RMD_{environment} API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Contraer todos los endpoints por defecto
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"RMD_{environment} API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Contraer todos los endpoints por defecto
    });
}

// Uso de middlewares
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<RenewTokenMiddleware>();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
