using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), new MySqlServerVersion(new Version(8, 0, 36)));
});


//מגדיר איזה פונקציות יהיו ב swagger
// builder.Services.AddCors(options =>
//         {
//             options.AddPolicy("MyPolicy", policy =>
//             {
//                 policy.WithMethods("GET", "POST", "PUT", "DELETE");
//                 policy.WithHeaders("*");
//             });
//         });


// Add services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
   
});

// Other service configurations...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//get
app.MapGet("/items", async (ToDoDbContext context) =>
{
    var tasks = await context.Items.ToListAsync();
    return tasks;
});

//add
app.MapPost("/items", async (ToDoDbContext context, Item item)=>
{
    context.Items.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

//update
app.MapPut("/items/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    var existingItem = await context.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound("Item not found");
    }

    // Update the existing item with the new data
    existingItem.Name = updatedItem.Name;
    // Update other properties as needed

    await context.SaveChangesAsync();

    return Results.NoContent();
});


//delete
app.MapDelete("/items/{id}", async (ToDoDbContext context, int id) => 
{
    var existingItem = await context.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound("Item not found");
    }

    // Remove the item from the context and save changes
    context.Items.Remove(existingItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline
app.UseRouting();

app.UseCors("AllowAnyOrigin");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    // options.SerializeAsV2 = true;
});

app.Run();