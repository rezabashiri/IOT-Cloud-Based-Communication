using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Infrastructure.Swagger.Filters;

public class RemoveVersionFromParameterFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation,
        OperationFilterContext context)
    {
        if (operation.Parameters.Count == 0 || operation.Parameters.All(p => p.Name != "version"))
        {
            return;
        }

        var versionParameter = operation.Parameters.Single(p => p.Name == "version");
        operation.Parameters.Remove(versionParameter);
    }
}