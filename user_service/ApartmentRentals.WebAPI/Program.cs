using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.kafka;
using ApartmentRentals.WebAPI.Services;
using ApartmentRentals.WebAPI.Services.GraphQL;
using ApartmentRentals.WebAPI.Services.Repositories;
using Confluent.Kafka;
using kafka.KafkaProducer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Prometheus;
using StackExchange.Redis;
using UserServiceMetrics;
using KafkaSettings;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ApartmentRentals.Tests")]

var builder = WebApplication.CreateBuilder(args);

// === Регистрируем кафку ===
builder.Services.AddOptions<KafkaProducerSettings>()
    .Bind(builder.Configuration.GetSection("KafkaProducer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var kafkaProducerSettings = sp.GetRequiredService<IOptions<KafkaProducerSettings>>().Value;
    var config = new ProducerConfig
    {
        BootstrapServers = kafkaProducerSettings.BootstrapServers,
        Acks = kafkaProducerSettings.Acks,
        EnableIdempotence = kafkaProducerSettings.EnableIdempotence,
    };

    var producerBuilder = new ProducerBuilder<string, string>(config);
    producerBuilder.SetErrorHandler((_, error) =>
    {
        var logger = sp.GetRequiredService<ILogger<IProducer<string, string>>>();
        logger.LogError("Kafka producer error: {Error}", error.Reason);
    });

    return producerBuilder.Build();
});
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddOptions<KafkaConsumerSettings>()
    .Bind(builder.Configuration.GetSection("KafkaConsumer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddHostedService<KafkaConsumer>();

builder.Services.AddScoped<UserService>();

// === Регистрируем бд ===
builder.Services.Configure<MongoDbContext>(
    builder.Configuration.GetSection("UsersDatabase"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(sp.GetRequiredService<IOptions<MongoDbContext>>().Value.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = "ApartmentRentalsUsers";
    return client.GetDatabase(databaseName);
});

builder.Services.AddScoped<IRepository<User>, MongoDbRepository<User>>();

// === регистрируем кэш ===
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCache:Configuration"];
    options.InstanceName = builder.Configuration["RedisCache:InstanceName"];
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["RedisCache:Configuration"]!));

// === Регистрация GraphQL ===
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// === регистрация метрик ===
builder.Services.AddMetrics();
builder.Services.AddHostedService<MetricsUpdateService>();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthorization();

app.UseHttpMetrics();

app.MapMetrics();

app.MapGraphQL(path: "/graphql");

app.Run();

public partial class Program { }



