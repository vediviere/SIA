using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Contracts.Requests.Users;
using SIA.IdentityService.Contracts.Responses.Users;
using SIA.IdentityService.Domain.Enums;

namespace SIA.IdentityService.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
  private readonly SetInitialPasswordUseCase _setInitialPasswordUseCase;
  private readonly LoginUseCase _loginUseCase;
  private readonly RefreshUseCase _refreshUseCase;
  private readonly LogoutUseCase _logoutUseCase;
  private readonly ChangePasswordUseCase _changePasswordUseCase;
  private readonly CreateStaffUserUseCase _createStaffUserUseCase;
  private readonly AssignRoleUseCase _assignRoleUseCase;
  private readonly RevokeRoleUseCase _revokeRoleUseCase;
  private readonly ChangeUserStatusUseCase _changeUserStatusUseCase;

  public UsersController(SetInitialPasswordUseCase setInitialPasswordUseCase, LoginUseCase loginUseCase, RefreshUseCase refreshUseCase, LogoutUseCase logoutUseCase, ChangePasswordUseCase changePasswordUseCase, CreateStaffUserUseCase createStaffUserUseCase, AssignRoleUseCase assignRoleUseCase, RevokeRoleUseCase revokeRoleUseCase, ChangeUserStatusUseCase changeUserStatusUseCase)
  {
    _setInitialPasswordUseCase = setInitialPasswordUseCase;
    _loginUseCase = loginUseCase;
    _refreshUseCase = refreshUseCase;
    _logoutUseCase = logoutUseCase;
    _changePasswordUseCase = changePasswordUseCase;
    _createStaffUserUseCase = createStaffUserUseCase;
    _assignRoleUseCase = assignRoleUseCase;
    _revokeRoleUseCase = revokeRoleUseCase;
    _changeUserStatusUseCase = changeUserStatusUseCase;
  }

  [AllowAnonymous]
  [HttpPost("initial-password")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> SetInitialPassword([FromBody] SetInitialPasswordRequest request, CancellationToken cancellationToken)
  {
    await _setInitialPasswordUseCase.ExecuteAsync(request, Guid.NewGuid(), cancellationToken);

    return NoContent();
  }

  [AllowAnonymous]
  [HttpPost("login")]
  [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var response = await _loginUseCase.ExecuteAsync(request, cancellationToken);

    return Ok(response);
  }

  [AllowAnonymous]
  [HttpPost("refresh")]
  [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
  {
    var response = await _refreshUseCase.ExecuteAsync(request, cancellationToken);

    return Ok(response);
  }

  [AllowAnonymous]
  [HttpPost("logout")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
  {
    await _logoutUseCase.ExecuteAsync(request, cancellationToken);

    return NoContent();
  }

  [Authorize]
  [HttpPut("password")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
  {
    var userId = GetAuthenticatedUserId();

    await _changePasswordUseCase.ExecuteAsync(userId, request, Guid.NewGuid(), cancellationToken);

    return NoContent();
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpPost("staff")]
  [ProducesResponseType(typeof(CreateStaffUserResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<ActionResult<CreateStaffUserResponse>> CreateStaffUser([FromBody] CreateStaffUserRequest request, CancellationToken cancellationToken)
  {
    var administratorUserId = GetAuthenticatedUserId();
    var tenantId = GetTenantId();

    var response = await _createStaffUserUseCase.ExecuteAsync(request, tenantId, administratorUserId, Guid.NewGuid(), cancellationToken);

    return StatusCode(StatusCodes.Status201Created, response);
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpPost("{userId:guid}/roles")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
  {
    var administratorUserId = GetAuthenticatedUserId();
    var tenantId = GetTenantId();

    await _assignRoleUseCase.ExecuteAsync(userId, request, tenantId, administratorUserId, Guid.NewGuid(), cancellationToken);

    return NoContent();
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpDelete("{userId:guid}/roles/{roleCode}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> RevokeRole(Guid userId, string roleCode, CancellationToken cancellationToken)
  {
    var administratorUserId = GetAuthenticatedUserId();
    var tenantId = GetTenantId();

    await _revokeRoleUseCase.ExecuteAsync(userId, roleCode, tenantId, administratorUserId, Guid.NewGuid(), cancellationToken);

    return NoContent();
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpPut("{userId:guid}/lock")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Lock(Guid userId, CancellationToken cancellationToken)
  {
    await ChangeStatus(userId, UserStatus.Locked, cancellationToken);

    return NoContent();
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpPut("{userId:guid}/activate")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Activate(Guid userId, CancellationToken cancellationToken)
  {
    await ChangeStatus(userId, UserStatus.Active, cancellationToken);

    return NoContent();
  }

  [Authorize(Policy = "Users.Manage")]
  [HttpPut("{userId:guid}/deactivate")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Deactivate(Guid userId, CancellationToken cancellationToken)
  {
    await ChangeStatus(userId, UserStatus.Inactive, cancellationToken);

    return NoContent();
  }

  private async Task ChangeStatus(Guid userId, UserStatus status, CancellationToken cancellationToken)
  {
    var administratorUserId = GetAuthenticatedUserId();
    var tenantId = GetTenantId();

    await _changeUserStatusUseCase.ExecuteAsync(userId, status, tenantId, administratorUserId, Guid.NewGuid(), cancellationToken);
  }

  private Guid GetAuthenticatedUserId()
  {
    var userIdValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    if (!Guid.TryParse(userIdValue, out var userId))
    {
      throw new UnauthorizedAccessException("El identificador del usuario autenticado no es válido.");
    }

    return userId;
  }

  private Guid GetTenantId()
  {
    var tenantIdValue = User.FindFirst("tenant_id")?.Value;

    if (!Guid.TryParse(tenantIdValue, out var tenantId))
    {
      throw new UnauthorizedAccessException("El identificador de la institución no es válido.");
    }

    return tenantId;
  }
}
