using WebKafka.Messaging.Implementations;
using WebKafka.Messaging.Interfaces;
using WebKafka.Services;
using WebKafka.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IEventBus, KafkaEventBus>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
