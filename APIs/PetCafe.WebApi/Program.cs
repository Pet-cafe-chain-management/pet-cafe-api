using Hangfire;
using PetCafe.Application;
using PetCafe.Application.GlobalExceptionHandling;
using PetCafe.WebApi;
using PetCafe.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<AppSettings>() ?? throw new Exception("Null configuration");
builder.Services.AddSingleton(configuration);
builder.Services.AddWebApiService(configuration);
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire");

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
