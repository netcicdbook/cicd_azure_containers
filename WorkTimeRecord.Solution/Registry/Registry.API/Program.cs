using Carter;
using Components.Communication.MessageBroker;
using Microsoft.EntityFrameworkCore;
using Registry.API.Data.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Registry Api",
        Description = "API de �ltimo registro horario de cada empleado",
        Version = "v1"
    });
});

builder.Services.AddCarter();
builder.Services.AddMediatR(configuration => { configuration.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddScoped(typeof(IRegistryContext), typeof(RegistryPostgresContext));

builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddDbContext<RegistryPostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserRecord")));

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");//CORS

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registry V1"));

app.MapCarter();

app.Run();
