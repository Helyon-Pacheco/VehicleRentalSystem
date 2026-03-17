using VehicleRentalSystem.Identity.ViewModels;

namespace VehicleRentalSystem.Identity.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerUser">The user registration details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
    Task<bool> RegisterAsync(RegisterUserViewModel registerUser);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="loginUser">The user login details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the login response.</returns>
    Task<LoginResponseViewModel> LoginAsync(LoginUserViewModel loginUser);

    /// <summary>
    /// Adds a new role.
    /// </summary>
    /// <param name="roleName">The name of the role to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
    Task<bool> AddRoleAsync(string roleName);

    /// <summary>
    /// Assigns roles and claims to a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="roles">The roles to assign.</param>
    /// <param name="claims">The claims to assign.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
    Task<bool> AssignRolesAndClaimsAsync(string userId, IEnumerable<string> roles, IEnumerable<ClaimViewModel> claims);

    /// <summary>
    /// Generates a JWT for a user.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the login response.</returns>
    Task<LoginResponseViewModel> GenerateJwtAsync(string email);
}
