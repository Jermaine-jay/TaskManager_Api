using TaskManager.Api.Extensions;
using TaskManager.Data.Seeds;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.RegisterDbContext(connectionString);

builder.Services.RegisterServices();
builder.Services.ConfigurationBinder(builder.Configuration);

builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureIISIntegration();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.ConfigureException(builder.Environment);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.SeedRole();

app.Run();
