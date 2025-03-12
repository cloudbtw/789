using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectX API", Version = "v1" });
});

// Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectX API v1"));
}

var projects = new List<ProjectX>();
var idCounter = 0; 

app.MapGet("/projects", () => Results.Ok(projects));

app.MapGet("/projects/{id}", (int id) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);
    if (project != null)
    {
        return Results.Ok(project);
    }
    return Results.NotFound();
});

app.MapPost("/projects", (ProjectX project) =>
{
    project.Id = idCounter++; 
    projects.Add(project);
    return Results.Created($"/projects/", project);
});

app.MapPut("/projects/{id}", (int id, ProjectX updatedProject) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);
    if (project != null)
    {
        project.Name = updatedProject.Name;
        project.Description = updatedProject.Description;
        project.Price = updatedProject.Price;
        project.Phone = updatedProject.Phone;
        return Results.Ok(project);
    }
    return Results.NotFound();
});

app.MapDelete("/projects/{id}", (int id) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);
    if (project != null)
    {
        projects.Remove(project);
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();

class ProjectX
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public string Phone { get; set; }
}