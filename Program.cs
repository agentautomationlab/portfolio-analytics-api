using Microsoft.EntityFrameworkCore;
using PortfolioAnalyticsApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure CORS for your portfolio website
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPortfolio", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173", // Vite dev server
            "http://localhost:3000",
            "https://your-portfolio-domain.com" // Replace with your actual domain
        )
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowPortfolio");
app.UseAuthorization();
app.MapControllers();

app.Run();
