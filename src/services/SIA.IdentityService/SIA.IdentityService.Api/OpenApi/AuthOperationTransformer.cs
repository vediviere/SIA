using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SIA.IdentityService.Api.OpenApi;

public sealed class AuthOperationTransformer : IOpenApiOperationTransformer
{
  public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
  {
    var metadata = context.Description.ActionDescriptor.EndpointMetadata;

    var allowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
    var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any();

    if (allowAnonymous || !requiresAuthorization || context.Document is null)
      return Task.CompletedTask;

    operation.Security ??= [];

    operation.Security.Add(new OpenApiSecurityRequirement
    {
      [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
    });

    return Task.CompletedTask;
  }
}
