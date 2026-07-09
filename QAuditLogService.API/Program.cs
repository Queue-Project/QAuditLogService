using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using QAuditLogService.Application;
using QAuditLogService.Application.Consumers.BranchService.BranchServiceConsumers;
using QAuditLogService.Application.Consumers.BranchService.CompanyConsumers;
using QAuditLogService.Application.Consumers.BranchService.CompanyServiceConsumers;
using QAuditLogService.Application.Consumers.QueueService.ComplaintConsumers;
using QAuditLogService.Application.Consumers.QueueService.QueueConsumers;
using QAuditLogService.Application.Consumers.QueueService.ReviewConsumers;
using QAuditLogService.Application.Consumers.UserService;
using QAuditLogService.Application.Consumers.UserService.BlockedCustomerConsumers;
using QAuditLogService.Application.Consumers.UserService.CustomerConsumers;
using QAuditLogService.Application.Consumers.UserService.EmployeeConsumers;
using QAuditLogService.Application.Interfaces;
using QAuditLogService.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuditEventConsumer>();
    x.AddConsumer<CustomerCreatedEventConsumer>();
    x.AddConsumer<CustomerUpdatedEventConsumer>();
    x.AddConsumer<CustomerDeletedEventConsumer>();
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeUpdatedEventConsumer>();
    x.AddConsumer<EmployeeDeletedEventConsumer>();
    x.AddConsumer<BlockedCustomerCreatedEventConsumer>();
    x.AddConsumer<BlockedCustomerDeletedEventConsumer>();
    x.AddConsumer<CompanyCreatedEventConsumer>();
    x.AddConsumer<CompanyUpdatedEventConsumer>();
    x.AddConsumer<CompanyDeletedEventConsumer>();
    x.AddConsumer<BranchCreatedEventConsumer>();
    x.AddConsumer<BranchUpdatedEventConsumer>();
    x.AddConsumer<BranchDeletedEventConsumer>();
    x.AddConsumer<CompanyServiceCreatedEventConsumer>();
    x.AddConsumer<CompanyServiceDeletedEventConsumer>();
    x.AddConsumer<CompanyServiceUpdatedEventConsumer>();
    x.AddConsumer<QueueEventConsumer>();
    x.AddConsumer<ReviewCreatedEventConsumer>();
    x.AddConsumer<ComplaintCreatedEventConsumer>();
    x.AddConsumer<ComplaintUpdatedEventConsumer>();


    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetService<IConfiguration>();

        var host = configuration?["RabbitMQ:Host"] ?? "queue-rabbitmq";
        var port = configuration?.GetValue<ushort?>("RabbitMQ:Port") ?? 5672;
        var username = configuration?["RabbitMQ:Username"] ?? "guest";
        var password = configuration?["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddApplicationService();
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


if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName== "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();