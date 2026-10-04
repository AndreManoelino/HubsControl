using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using ControlHub.Api.Data;
using ControlHub.Api.Services;
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURAÇÕES
// ==========================================

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(databaseUrl))
{
    throw new InvalidOperationException(
        "A variável DATABASE_URL não foi encontrada no arquivo .env.");
}

var databaseUri = new Uri(databaseUrl);

var userInfo = databaseUri.UserInfo.Split(':', 2);

var databaseUser = Uri.UnescapeDataString(userInfo[0]);
var databasePassword = Uri.UnescapeDataString(userInfo[1]);

var databaseName = databaseUri.AbsolutePath.TrimStart('/');

var connectionString =
    $"Host={databaseUri.Host};" +
    $"Port=5432;" +
    $"Database={databaseName};" +
    $"Username={databaseUser};" +
    $"Password={databasePassword};" +
    $"SSL Mode=Require;" +
    $"Channel Binding=Require;";

// ==========================================
// BANCO DE DADOS - NEON POSTGRESQL
// ==========================================
builder.Services.AddDbContext<ControlHubDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// ==========================================
// CONTROLLERS
// ==========================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddControllers();

// ==========================================
// CORS
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("ControlHubPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==========================================
// JWT
// ==========================================

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "A variável JWT_SECRET não foi encontrada no arquivo .env.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSecret)
            ),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// SWAGGER
// ==========================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ControlHub API",
        Version = "v1",
        Description = "API do sistema ControlHub"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT. Exemplo: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ==========================================
// APPLICATION
// ==========================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ControlHubPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
await MasterInitializer.CriarMasterAsync(app.Services);
app.Run();