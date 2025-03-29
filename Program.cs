using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Operacao.Context;
using Manutencao.Interfaces;
using Manutencao.Entities;
using Manutencao.DTOs;
using Manutencao.Controllers;
using Manutencao.ModelViews;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manutencao.Servicos;

var builder = WebApplication.CreateBuilder(args);


// Registre a implementação do serviço para iAdministradorServico
builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


// Configurar autenticação usando JWT
/*var key = builder.Configuration["Jwt:AdmRoot"];
var issuer = builder.Configuration["Jwt:Edson"];
var audience = builder.Configuration["Jwt:Audience"];*/
var tokenValidityInMinutes = int.Parse(builder.Configuration["Jwt:TokenValidityInMinutes"]);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "AdmRoot";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();

// Adicionar autorização
/*builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdmPolicy", policy =>
        policy.RequireRole("Administrador")); // Use the string value of the enum
});*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorizartion",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<OperacaoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));


string GerarTokenJwt(Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim(JwtRegisteredClaimNames.Sub, administrador.Matricula),
        new Claim("Perfil", administrador.Perfil.ToString()), // Convert to string
        new Claim(ClaimTypes.Role, administrador.Perfil.ToString()), // Convert to string
        new Claim(ClaimTypes.Role, administrador.Perfil.ToString() == "1" ? "Administrador" : "Usuario" ), 
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

var app = builder.Build();



app.MapPost("/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) => {
    var administrador = administradorServico.Login(loginDTO);
    if (administrador != null)
    {
        string token = GerarTokenJwt(administrador);

        return Results.Ok(new AdmLogado
        {
            Matricula = administrador.Matricula,
            Perfil = administrador.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Login");

app.MapGet("/administradores", ([FromQuery] int? pagina, iAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModel>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var administrador in administradores)
    {
        adms.Add(new AdministradorModel
        {
            Id = administrador.Id,
            Matricula = administrador.Matricula,
            Perfil = administrador.Perfil
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador" }) // Use the string value of the enum
.WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);
    if (administrador == null) return Results.NotFound();
    return Results.Ok(new AdministradorModel
        {
            Id = administrador.Id,
            Matricula = administrador.Matricula,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador" }) // Use the string value of the enum
.WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, iAdministradorServico administradorServico) => {
    // Remove the validation here and move it to the service
    var administrador = new Administrador{
        Matricula = administradorDTO.Matricula,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil
    };

    try{
        administradorServico.Incluir(administrador);
    }catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }

    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModel
        {
            Id = administrador.Id,
            Matricula = administrador.Matricula,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador" }) // Use the string value of the enum
.WithTags("Administradores");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}/*
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}*/

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
