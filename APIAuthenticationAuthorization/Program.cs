global using Microsoft.EntityFrameworkCore;
global using APIAuthenticationAuthorization.UserDetails;
global using APIAuthenticationAuthorization.Context;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// --------------- ADD FOR DATABASE 
// ------ START

builder.Services.AddDbContext<DBContext>();

// --------------- ADD FOR DATABASE 
// ------ END






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
