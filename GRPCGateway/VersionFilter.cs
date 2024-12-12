using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GRPCGateway
{
    internal class VersionFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var removePaths = swaggerDoc.Paths.Where(x => !x.Key.Contains(swaggerDoc.Info.Version));

            foreach (var removePath in removePaths)
            {
                swaggerDoc.Paths.Remove(removePath.Key);
            }
        }
    }
}
