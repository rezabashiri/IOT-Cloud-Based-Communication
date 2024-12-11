using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Shared.Infrastructure.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Shared.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSharedInfrastructure(this IApplicationBuilder app,
        IWebHostEnvironment environment,
        string swaggerEndpoint = "swagger")
    {
        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseSwaggerDocumentation(environment, swaggerEndpoint);

        return app;
    }

    private static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app,
        IWebHostEnvironment environment,
        string swaggerEndpoint)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger(options => { options.RouteTemplate = $"{swaggerEndpoint}/{{documentName}}/swagger.json"; });
            app.UseSwaggerUI(
                options =>
                {
                    options.DefaultModelsExpandDepth(-1);
                    options.SwaggerEndpoint($"/{swaggerEndpoint}/v1/swagger.json", "v1");
                    options.SwaggerEndpoint($"/{swaggerEndpoint}/v2/swagger.json", "v2");
                    options.RoutePrefix = swaggerEndpoint;
                    options.DisplayRequestDuration();
                    options.DocExpansion(DocExpansion.None);
                });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        return app;
    }
}