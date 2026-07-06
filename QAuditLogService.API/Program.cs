using Microsoft.EntityFrameworkCore;
using Npgsql;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuditLogDbContext, AuditLogDbContext>();
builder.Services.AddDbContext<AuditLogDbContext>(options =>
{
    var dataSourceBuilder =
        new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
    dataSourceBuilder.EnableDynamicJson();
    var datasource = dataSourceBuilder.Build();
    options.UseNpgsql(datasource);
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();