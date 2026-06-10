using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserLogin operations.
    /// Handles credential lookup, refresh token management, and user creation.
    /// </summary>
    public interface IUserLoginRepository
    {
        // Find a user by their username — used during login
        Task<UserLogin?> GetByUsernameAsync(string username);

        // Find a user by their refresh token — used during token refresh
        Task<UserLogin?> GetByRefreshTokenAsync(string refreshToken);

        // Find a user by their ID — used for logout and token revocation
        Task<UserLogin?> GetByIdAsync(int userId);

        // Save changes to an existing user login (e.g., update refresh token, last login)
        Task UpdateAsync(UserLogin userLogin);

        // Create a new user login record — Admin only
        Task AddAsync(UserLogin userLogin);

        // Check if a username is already taken — used before creating a new login
        Task<bool> UsernameExistsAsync(string username);

        // Check if an employee record exists — used when creating a login for an employee
        Task<bool> EmployeeExistsAsync(int employeeId);
    }
}
