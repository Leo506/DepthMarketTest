using DepthMarketTest.Base;
using DepthMarketTest.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("mongo");

builder.Services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

builder.Services.AddSingleton<IMongoDbContext<OrderModel>>(provider => new MongodbContext<OrderModel>(
    new MongodbSettings()
    {
        ConnectionString = connectionString,
        CollectionName = builder.Configuration["Person:Collection"],
        DbName = builder.Configuration["Person:Database"]
    }, provider.GetRequiredService<IMongoClient>()));

builder.Services.AddSingleton<MongodbWorker<OrderModel>>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
