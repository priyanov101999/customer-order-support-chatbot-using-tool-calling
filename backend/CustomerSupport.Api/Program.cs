using CustomerSupport.Api.Data;
using CustomerSupport.Api.Repositories;
using DotNetEnv;
using CustomerSupport.Api.Services; 
Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<ICustomerSupportRepository, CustomerSupportRepository>();
builder.Services.AddScoped<ILlmChatOrchestrator, OpenAiChatOrchestrator>();
builder.Services.AddSingleton<IChatMemoryService, InMemoryChatMemoryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();