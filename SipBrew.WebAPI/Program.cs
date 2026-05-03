using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SipBrew.Core;
using SipBrew.Core.Business;
using SipBrew.Core.Contracts;
using SipBrew.Core.DTO;
using SipBrew.Core.Migrations;
using SipBrew.Core.Models;
using SipBrew.Core.Services;
using SipBrew.Core.Validators;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configuration
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

//Dependency Injection
builder.Services.AddScoped<IProductsBL, ProductsBL>();

builder.Services.AddScoped<IValidator<AddProductDTO>, AddProductValidator>();
builder.Services.AddScoped<IValidator<ProductsModel>, UpdateProductValidator>();

//Controller and API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//JWT
builder.Services.AddScoped<JWTService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();