using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateSprintExistsAttribute : TypeFilterAttribute
{
  public ValidateSprintExistsAttribute():base(typeof
    (ValidateSprintExistsFilterImpl))
  {
  }
  private class ValidateSprintExistsFilterImpl : IAsyncActionFilter
  {
    private readonly ISprintsRepository _sprintRepository;
    public ValidateSprintExistsFilterImpl(ISprintsRepository sprintRepository)
    {
        _sprintRepository = sprintRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("sprintId"))
      {
        var id = context.ActionArguments["sprintId"] as int?;
        if (id.HasValue)
        {
          if (await _sprintRepository.GetSprint(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The Sprint was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}