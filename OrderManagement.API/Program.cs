using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderManagement.Application;
using OrderManagement.Infrastructure;
using OrderManagement.Infrastructure.Authentication;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// DI For Infrastructure Layer
builder.Services.AddInfrastructure(
    builder.Configuration);
builder.Services.AddJwtAuthentication(
    builder.Configuration);
// DI For Application Layer
builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//Seed Data
await app.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();