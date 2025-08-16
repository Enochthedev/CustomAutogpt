using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GemsAi.Core.Models;
using GemsAi.Core.Agent;
using GemsAi.Core.Ai;
using Microsoft.AspNetCore.Http;


namespace GemsAi.Web
{
    public static class WebServer
    {
        public static async Task StartAsync(string[] args, IServiceCollection baseServices)
        {
            var builder = WebApplication.CreateBuilder(args);

            foreach (var service in baseServices)
            {
                builder.Services.Add(service);
            }

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            AiEndpoints.Map(app);
            ChatEndpoints.Map(app);

            Console.WriteLine("🌐 Web server running on http://localhost:5000/swagger");
            await app.RunAsync("http://localhost:5000");
        }
    }
}