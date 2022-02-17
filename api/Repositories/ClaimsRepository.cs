using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

public class ClaimsRepository : IClaimsRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ClaimsRepository(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IdentityResult> CreateClaim(User user, string ClaimType, string ClaimValue)
    {
        var newClaim = new Claim(ClaimType, ClaimValue);

        return await _userManager.AddClaimAsync(user, newClaim);
    }

    public async Task<IdentityResult> CreateClaimForUsers(string type, string value, List<User> users)
    {
        var newClaim = new Claim(type, value);

        foreach (var user in users)
        {
            await _userManager.AddClaimAsync(user, newClaim);
        }

        return IdentityResult.Success;
    }

    public Task<ICollection<Claim>> GetUserClaims(int userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> RemoveClaim(List<string> ClaimValues)
    {
        throw new System.NotImplementedException();
    }
}