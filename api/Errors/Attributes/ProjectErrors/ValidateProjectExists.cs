using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateProjectExistsAttribute : TypeFilterAttribute
{
  public ValidateProjectExistsAttribute():base(typeof
    (ValidateProjectExistsFilterImpl))
  {
  }
  private class ValidateProjectExistsFilterImpl : IAsyncActionFilter
  {
    private readonly IProjectsRepository _projectRepository;
    public ValidateProjectExistsFilterImpl(IProjectsRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("projectId"))
      {
        var id = context.ActionArguments["projectId"] as int?;
        if (id.HasValue)
        {
          if (await _projectRepository.GetProject(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The Project was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}