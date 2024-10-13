using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StartspelerAPI.Data;
using StartspelerAPI.Data.Repository;
using StartspelerAPI.Data.UnitOfWork;
using StartspelerAPI.Helper;
using StartspelerAPI.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//0000000000000000000000000000000000000
// We gebruiken de MySettings in Token Class, de Login methode heeft deze nodig
// Token overpompen in MySettings
Token.mySettings = new MySettings
{
    Secret = (builder.Configuration["MySettings:Secret"] ?? "d4f.5E6a7-8b9c-0d1e-WfGl1m-4h5i6j7k8l9m").ToCharArray(),
    ValidIssuer = builder.Configuration["MySettings:ValidIssuer"] ?? "https://localhost:7055",
    ValidAudience = builder.Configuration["MySettings:ValidAudience"] ?? "https://localhost:7055"
};

builder.Configuration.GetRequiredSection(nameof(MySettings)).Bind(Token.mySettings);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa
var audiences = Token.mySettings.ValidAudiences;

// Authentication toevoegen
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Jwt Bearer toevoegen
.AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true; // debugging
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.UseSecurityTokenValidators = true; // fix bug 8.0
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateLifetime = false, // onderdrukken bug 8.0
        //ValidateIssuer = true,
        //ValidateAudience = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        //ValidAudiences = audiences,
        //ValidIssuer = Token.mySettings.ValidIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Token.mySettings.Secret))
    };
});

//BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBbbb


builder.Services.AddEndpointsApiExplorer();
// CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
builder.Services.AddSwaggerGen(swagger => {
    //This is to generate the Default UI of Swagger Documentation    
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AuthTest Web API",
        Description = "Authentication and Authorization in ASP.NET with JWT and Swagger"
    });
    // To Enable authorization using Swagger (JWT)    
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter ZONDER Bearer. Dus gewoon je TOKEN: \r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
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

//DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDdd
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// db service
builder.Services.AddDbContext<StartspelerAPIContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDBConnection")));

// service uow
builder
    .Services
    .AddScoped<IUnitOfWork>();



var app = builder.Build();

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
