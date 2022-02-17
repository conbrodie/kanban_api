
using api.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Repositories
{
    public interface IClaimsRepository
    {
        Task<ICollection<Claim>> GetUserClaims(int userId);
        Task<IdentityResult> CreateClaim(User user, string ClaimType, string ClaimValue);
        Task<IdentityResult> CreateClaimForUsers(string type, string value, List<User> users);
        Task<bool> RemoveClaim(List<string> ClaimValues);
    }
}