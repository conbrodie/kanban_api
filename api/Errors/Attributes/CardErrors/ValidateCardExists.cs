using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateCardExistsAttribute : TypeFilterAttribute
{
  public ValidateCardExistsAttribute():base(typeof
    (ValidateCardExistsFilterImpl))
  {
  }
  private class ValidateCardExistsFilterImpl : IAsyncActionFilter
  {
    private readonly ICardRepository _cardRepository;
    public ValidateCardExistsFilterImpl(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("cardId"))
      {
        var id = context.ActionArguments["cardId"] as int?;
        if (id.HasValue)
        {
          if (await _cardRepository.GetCard(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The card was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}