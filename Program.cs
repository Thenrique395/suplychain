using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;

using SupplyChain.Infrastructure;
using SupplyChain.Application;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// LOGGING (SERILOG)
// --------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --------------------------------------------------
// CONFIGURATION
// --------------------------------------------------
var configuration = builder.Configuration;

var connectionString =
    configuration.GetValue<string>("ConnectionStrings:Default")
    ?? "Host=db;Port=5432;Database=employees;Username=postgres;Password=Pass@123";

// --------------------------------------------------
// DATABASE
// --------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// --------------------------------------------------
// CONTROLLERS + VALIDATION
// --------------------------------------------------
builder.Services.AddControllers()
    .AddFluentValidation(cfg =>
        cfg.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --------------------------------------------------
// DEPENDENCY INJECTION
// --------------------------------------------------
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SupplyChain API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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
// --------------------------------------------------
// AUTHENTICATION (JWT)
// --------------------------------------------------
var jwtKey =
    configuration.GetValue<string>("Jwt:Key")
    ?? "VerySecretKey_DoNotUseInProd_ChangeMe";

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// --------------------------------------------------
// CORS (ANGULAR FRONTEND)
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --------------------------------------------------
// HOST / PORT
// --------------------------------------------------
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// --------------------------------------------------
// DATABASE MIGRATIONS (RETRY SAFE)
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const int maxRetries = 10;
    var attempt = 0;

    while (true)
    {
        try
        {
            Console.WriteLine($"Applying migrations... Attempt {attempt + 1}/{maxRetries}");
            db.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            attempt++;

            if (attempt >= maxRetries)
                throw;

            Console.WriteLine($"Migration failed: {ex.Message}");
            Console.WriteLine("Retrying in 5 seconds...");
            Thread.Sleep(5000);
        }
    }
}

// --------------------------------------------------
// MIDDLEWARE PIPELINE (ORDER MATTERS)
// --------------------------------------------------
app.UseSerilogRequestLogging();

app.UseRouting();

app.UseCors("Frontend"); // ⬅️ CORS ANTES de Auth

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
