using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserRepository(AppDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersAsync(List<int> userId)
        {
            return await _context.Users.Where(u => userId.Contains(u.Id)).ToListAsync();
        }

        public async Task<ICollection<UserDTO>> Search(string searchQuery, List<int> userId)
        {

            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(u => u.FirstName.Contains(searchQuery) || u.LastName.Contains(searchQuery));          
            }

            // filter out users that have already been selected
            var filteredUsers = await query.Where(u => !userId.Contains(u.Id)) 
                                    .OrderBy(u => u.LastName)
                                    .AsNoTracking()
                                    .ToListAsync();

            var users = _mapper.Map<List<UserDTO>>(filteredUsers);

            return users;
        }
    }
}
