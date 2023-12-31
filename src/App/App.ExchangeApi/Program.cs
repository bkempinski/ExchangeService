using App.ExchangeApi;

var builder = WebApplication
    .CreateBuilder(args)
    .AddLogs()
    .AddImplementations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();