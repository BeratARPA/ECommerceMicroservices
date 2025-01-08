using Order.Infrastructure.Data;
using Order.Infrastructure.Interfaces;
using Order.Infrastructure.Repositories;
using Order.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ECommerce.Shared.Messaging;
using ECommerce.Shared.Events;
using Order.Infrastructure.EventHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Redis Cache Entegrasyonu
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "OrderCache_";
});

// Database Entegrasyonu
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// HTTP Client Registration
builder.Services.AddHttpClient<IProductHttpClient, ProductHttpClient>();

// RabbitMQ Registration
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<OrderCancelledEvent>, OrderCancelledEventHandler>();

builder.WebHost.UseUrls("http://*:9000");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

// RabbitMQ Subscription with retry logic
var retryCount = 0;
const int maxRetries = 5;

while (retryCount < maxRetries)
{
    try
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>(
            RabbitMQSettings.ExchangeName,
            RabbitMQSettings.OrderCreatedQueueName
        );

        eventBus.Subscribe<OrderCancelledEvent, OrderCancelledEventHandler>(
            RabbitMQSettings.ExchangeName,
            RabbitMQSettings.OrderCancelledQueueName
        );
        break;
    }
    catch
    {
        retryCount++;
        if (retryCount == maxRetries)
            throw;
        Thread.Sleep(5000); // 5 saniye bekle
    }
}

app.Run();
