using System.Diagnostics;

namespace PetCafe.WebApi.Middlewares
{
    public class PerformanceMiddleware(Stopwatch stopwatch) : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            stopwatch.Restart();
            stopwatch.Start();
            Console.WriteLine("start performance recored");
            await next(context);
            Console.WriteLine("end performance recored");
            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
        }
    }
}
