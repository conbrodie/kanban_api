using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateSprintListExistsAttribute : TypeFilterAttribute
{
  public ValidateSprintListExistsAttribute():base(typeof
    (ValidateSprintListExistsFilterImpl))
  {
  }
  private class ValidateSprintListExistsFilterImpl : IAsyncActionFilter
  {
    private readonly ISprintListRepository _sprintListRepository;
    public ValidateSprintListExistsFilterImpl(ISprintListRepository sprintListRepository)
    {
        _sprintListRepository = sprintListRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("sprintListId"))
      {
        var id = context.ActionArguments["sprintListId"] as int?;
        if (id.HasValue)
        {
          if (await _sprintListRepository.GetSprintList(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The Sprint list was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}