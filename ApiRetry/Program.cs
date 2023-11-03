using ApiRetry.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("AnotherProxyHttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
}).AddPolicyHandler(PolicyHelpers.GetPolicy());

builder.Services.AddHttpClient("YetAnotherProxyHttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
}).AddPolicyHandler(PolicyHelpers.GetAdvancedPolicy());

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
