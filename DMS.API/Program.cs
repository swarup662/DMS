using DMS.API.Helpers;
using DMS.API.Middleware;
using DMS.API.Services;
using DMS.API.Repositories;
using DMS.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using DMS.API.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Add services ---
builder.Services.AddSingleton<DBHelper>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddControllers();


// --- Register services ---
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<PermissionController>();

// --- API Versioning ---
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// --- Swagger ---
builder.Services.AddSwaggerGen(options =>
{
    // JWT Bearer support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[]{}
        }
    });

    // Dynamic SwaggerDoc per API version
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = "DMS.API",
            Version = description.ApiVersion.ToString()
        });
    }
});

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            $"DMS API {description.GroupName.ToUpperInvariant()}");
    }
});
app.UseMiddleware<ErrorLoggingMiddleware>();
// --- Swagger ---

// --- Middleware order ---
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<ActivityLoggingMiddleware>();

// --- Swagger ---
var apiProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            $"DMS.API {description.GroupName.ToUpperInvariant()}");
    }
    options.RoutePrefix = string.Empty; // Swagger opens at root /
});

// --- Map controllers ---
app.MapControllers();

// --- Default redirect to Swagger ---
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
