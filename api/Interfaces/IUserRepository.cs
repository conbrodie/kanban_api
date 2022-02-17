using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTO;
using api.Models;

namespace api.Repositories
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetUsersAsync(List<int> userId);
        Task<ICollection<UserDTO>> Search(string searchQuery, List<int> creatorId);
    }
}