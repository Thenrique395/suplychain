
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SupplyChain.Infrastructure;
using SupplyChain.Application;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Configuration and DB
var configuration = builder.Configuration;
//var conn = configuration.GetValue<string>("ConnectionStrings:Default") 
  //  ?? "Host=localhost;Database=employees;Username=postgres;Password=Pass@123";

  //var conn = configuration.GetValue<string>("ConnectionStrings:Default") ?? "Host=192.168.1.85;Port=5432;Database=employees;Username=postgres;Password=Pass@123";

  var conn = configuration.GetValue<string>("ConnectionStrings:Default") ?? "Host=db;Port=5432;Database=employees;Username=postgres;Password=Pass@123";

// Add DbContext using Npgsql
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(conn));

// FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Program>());

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key") ?? "VerySecretKey_DoNotUseInProd_ChangeMe";
var key = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Register validators assembly scanning (Application should contain validators)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
