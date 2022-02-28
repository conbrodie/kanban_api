using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateDepartmentExistsAttribute : TypeFilterAttribute
{
  public ValidateDepartmentExistsAttribute():base(typeof
    (ValidateDepartmentExistsFilterImpl))
  {
  }
  private class ValidateDepartmentExistsFilterImpl : IAsyncActionFilter
  {
    private readonly IDepartmentRepository _departmentRepository;
    public ValidateDepartmentExistsFilterImpl(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("departmentId"))
      {
        var id = context.ActionArguments["departmentId"] as int?;
        if (id.HasValue)
        {
          if (await _departmentRepository.GetDepartment(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The Department was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}