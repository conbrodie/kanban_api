using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateUserExistsAttribute : TypeFilterAttribute
{
  public ValidateUserExistsAttribute():base(typeof
    (ValidateUserExistsFilterImpl))
  {
  }
  private class ValidateUserExistsFilterImpl : IAsyncActionFilter
  {
    private readonly IUserRepository _userRepository;
    public ValidateUserExistsFilterImpl(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("userId"))
      {
        var id = context.ActionArguments["userId"] as int?;
        if (id.HasValue)
        {
          if (await _userRepository.GetUserByIdAsync(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The user was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}