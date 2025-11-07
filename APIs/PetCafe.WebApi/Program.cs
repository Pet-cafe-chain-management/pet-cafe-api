using Hangfire;
using PetCafe.Application;
using PetCafe.Application.GlobalExceptionHandling;
using PetCafe.Application.Services;
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

// Cấu hình cronjob cho DailySchedule
RecurringJob.AddOrUpdate<IDailyScheduleService>(
    "auto-assign-schedules",
    service => service.AutoAssignSchedulesAsync(),
    "0 0 1 * *", // Chạy vào 00:00 ngày đầu tiên của mỗi tháng
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    }
);

// Cấu hình cronjob cho DailyTask - AutoAssignTasks (mỗi thứ 2 hàng tuần)
RecurringJob.AddOrUpdate<IDailyTaskService>(
    "auto-assign-tasks",
    service => service.AutoAssignTasksAsync(),
    "0 0 * * 1", // Chạy vào 00:00 mỗi thứ 2
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    }
);

// Cấu hình cronjob cho DailyTask - AutoChangeStatus (mỗi cuối ngày)
RecurringJob.AddOrUpdate<IDailyTaskService>(
    "auto-change-task-status",
    service => service.AutoChangeStatusAsync(),
    "0 0 * * *", // Chạy vào 00:00 mỗi ngày (đầu ngày mới để cập nhật các task của ngày cũ)
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    }
);

// Cấu hình cronjob để cleanup expired orders (mỗi 5 phút)
RecurringJob.AddOrUpdate<IOrderService>(
    "cleanup-expired-orders",
    service => service.CleanupExpiredOrdersAsync(),
    "*/5 * * * *", // Chạy mỗi 5 phút
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    }
);

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
