using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Extensions;
using TaskManager.Data.Seeds;

var builder = WebApplication.CreateBuilder(args);


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.RegisterDbContext(connectionString);

builder.Services.RegisterServices();

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);



builder.Services.AddHostedService<ConsumeScopedServiceHostedService>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager Api", Version = "v1" });
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
             Array.Empty<string>()
        },
    });
});

builder.Services.AddHttpContextAccessor();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.ConfigureException(builder.Environment);
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.MapControllers();

await app.SeedRole();
await app.ClaimSeeder();
await app.SeededUserAsync();
await app.ProjectSeeder();

app.Run();