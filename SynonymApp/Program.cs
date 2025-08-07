using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SynonymApp.Application.Synonyms.Queries;
using SynonymApp.Domain;
using SynonymApp.Domain.Constants;
using SynonymApp.Extensions;
using SynonymApp.Infrastructure.ExceptionHandlers;
using System.Reflection;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var services = builder.Services;

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = "SynonymAuth",
        ValidAudience = "SynonymApp",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("b5498854e238943a22c1f8ccfd4d8523b72b6e8880520e2eb7e72dbc06a17887"))
    };
});

builder.Services.AddAuthorization();

services.AddDbContext<SynonymsDbContext>(options =>
    options.UseSqlite(
        $"Data Source = synonyms.db",
        x => x.MigrationsAssembly("SynonymApp.Infrastructure")));

services.AddControllers();

services.AddMinApiEndpoints(Assembly.GetExecutingAssembly());


services.AddEndpointsApiExplorer();

services.AddCustomVersioning();


services.AddMemoryCache();

services.AddHttpContextAccessor();

services.AddServicesAndRepositories();
services.AddCustomMediator(typeof(GetSynonymsQuery).Assembly);
services.AddValidatorsFromAssemblyContaining<GetSynonymsQueryValidator>();

services.AddProblemDetails();
services.AddExceptionHandler<GlobalExceptionHandler>();

services.AddCustomSwagger(
    xmlPath: Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"),
    title: "SynonymsApp",
    description: "SynonymsApp");


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SynonymsDbContext>();
    var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

    var stored = await dbContext.Synonyms.FirstOrDefaultAsync();
    if (stored is not null)
    {
        var graph = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(stored.GraphJson);
        if (graph is not null)
        {
            memoryCache.Set(CacheKeyConstants.CacheKey, graph);
        }
    }
    else
    {
        // If nothing in DB, start with an empty graph
        memoryCache.Set(CacheKeyConstants.CacheKey, new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase));
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapEndpoints();

app.Run();
