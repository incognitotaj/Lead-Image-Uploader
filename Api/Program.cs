
using Api.Extensions;
using Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Lead / Customer Images API",
        Description = "Lead / Customer Images API",
        TermsOfService = new Uri("https://github.com/incognitotaj"),
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://github.com/incognitotaj")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://github.com/incognitotaj")
        }
    });

    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddDbContext<ApplicationDbContext>((sp, optionsBuilder) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database");
    optionsBuilder.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

    await app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


namespace Api
{
    public class Program;
}