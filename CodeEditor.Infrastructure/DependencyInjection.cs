using CodeEditor.Appl.Interfaces;
using CodeEditor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodeEditor.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {

            services.AddScoped<IFileOperator, FileOperator>();

            services.AddScoped<Convertor>();

            /*services.AddScoped<Lexer>();
            services.AddScoped<Parser>();*/
            return services;
        }
    }
}
