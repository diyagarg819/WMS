using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IUserLoginRepository
    {
        Task<UserLogin?> GetByUsernameAsync(string username);
        Task<UserLogin?> GetByIdAsync(int userId);
        Task UpdateAsync(UserLogin userLogin);
        Task AddAsync(UserLogin userLogin);
        Task<bool> UsernameExistsAsync(string username);
    }
}
