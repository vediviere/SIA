using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SIA.SchoolControlService.Api.OpenApi;

public sealed class BearerTransformer(IAuthenticationSchemeProvider schemesProvider) : IOpenApiDocumentTransformer
{
  public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
  {
    var schemes = await schemesProvider.GetAllSchemesAsync();

    if (!schemes.Any(scheme => scheme.Name == "Bearer"))
    {
      return;
    }

    document.Components ??= new OpenApiComponents();

    document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
    {
      ["Bearer"] = new OpenApiSecurityScheme
      {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        BearerFormat = "JWT"
      }
    };
  }
}
