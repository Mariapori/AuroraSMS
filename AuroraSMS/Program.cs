using AuroraSMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.AddSecurityDefinition("API-Key", new OpenApiSecurityScheme()
    {
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "AuroraSMS API-Key"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "API-Key"
                },
                Scheme = "API-Key",
                Name = "X-API-Key",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.Configure<AuroraSMSConfig>(builder.Configuration.GetSection("AuroraSettings"));
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
builder.Services.AddCors(setup => setup.AddPolicy("public", c =>
{
    c.AllowAnyHeader();
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
}));

builder.Services.AddDbContext<AuroraSMSDbContext>(options =>
{
    string? host = Environment.GetEnvironmentVariable("dbhost");
    string? db = Environment.GetEnvironmentVariable("dbdb");
    string? dbuser = Environment.GetEnvironmentVariable("dbuser");
    string? dbpass = Environment.GetEnvironmentVariable("dbpass");
    string? dbport = Environment.GetEnvironmentVariable("dbport") ?? "3306";
    
    if(string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db) || string.IsNullOrEmpty(dbuser) || string.IsNullOrEmpty(dbpass))
    {
        string? connString = builder.Configuration.GetSection("AuroraSettings").GetValue<string>("ConnectionString");
        if(connString != null)
        {
            options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        }
    }
    else
    {
        string connString = $"Server={host};Port={dbport};Database={db};Uid={dbuser};Pwd={dbpass};";
        options.UseMySql(connString, ServerVersion.AutoDetect(connString));
    }

});

var app = builder.Build();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    //Automaattinen tietokantamigraatio
    scope?.ServiceProvider?.GetService<AuroraSMSDbContext>()?.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("public");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
