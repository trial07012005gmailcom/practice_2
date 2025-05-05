using System;
using System.IO;
using System.Reflection;
using ClinicManagement.API.Middleware;
using ClinicManagement.PatientManager.Repository;
using ClinicManagement.PatientManager.Repository.Interfaces;
using ClinicManagement.PatientManager.Services;
using ClinicManagement.PatientManager.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinic Management API",
        Version = "v1",
        Description = "A Web API for managing patients in a clinic",
        Contact = new OpenApiContact
        {
            Name = "Clinic Management Team",
            Email = "support@clinicmanagement.com"
        }
    });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    c.EnableAnnotations();
});

// Configuration for patient file path
var patientFilePath = Path.Combine(AppContext.BaseDirectory, "patients.txt");
builder.Configuration["PatientFilePath"] = patientFilePath;

// Add HTTP client for external API
builder.Services.AddHttpClient();

// Register repositories
builder.Services.AddSingleton<IPatientRepository>(provider => 
    new FilePatientRepository(
        builder.Configuration["PatientFilePath"],
        provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FilePatientRepository>>()
    )
);

// Register services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IGiftService, GiftService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

// Use global exception handling middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
