using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

public class ValidateUsersExistsAttribute : TypeFilterAttribute
{
  public ValidateUsersExistsAttribute():base(typeof
    (ValidateUsersExistsFilterImpl))
  {
  }
  private class ValidateUsersExistsFilterImpl : IAsyncActionFilter
  {
    private readonly AppDbContext _context;
    public ValidateUsersExistsFilterImpl(AppDbContext context)
    {
        _context = context;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("userId"))
      {
        var allUserIds = context.ActionArguments["userId"] as List<int>;
        var validUserIds = await _context.Users.Where(user => allUserIds.Contains(user.Id)).Select(user => user.Id).ToListAsync();

        if (allUserIds.Count != validUserIds.Count)
        {
            allUserIds.RemoveAll(id => validUserIds.Contains(id));
            AddModelErrors(context, allUserIds);
            return;
        } 
      }
      await next();
    }
    public void AddModelErrors(ActionExecutingContext context, List<int> ids)
    {
        foreach (var id in ids)
        {
            context.ModelState.AddModelError($"UserId: {id}", "User not found");
        }
        context.Result = new NotFoundError(context.ModelState);
    }
  }
}