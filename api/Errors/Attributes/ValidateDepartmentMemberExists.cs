using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateDepartmentMemberExistsAttribute : TypeFilterAttribute
{
  public ValidateDepartmentMemberExistsAttribute():base(typeof
    (ValidateDepartmentMemberExistsFilterImpl))
  {
  }
  private class ValidateDepartmentMemberExistsFilterImpl : IAsyncActionFilter
  {
    private readonly IDepartmentMemberRepository _departmentMemberRepository;
    public ValidateDepartmentMemberExistsFilterImpl(IDepartmentMemberRepository departmentMemberRepository)
    {
        _departmentMemberRepository = departmentMemberRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("departmentMemberId"))
      {
        var id = context.ActionArguments["departmentMemberId"] as int?;
        if (id.HasValue)
        {
          if (await _departmentMemberRepository.GetDepartmentMember(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The Department member was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}