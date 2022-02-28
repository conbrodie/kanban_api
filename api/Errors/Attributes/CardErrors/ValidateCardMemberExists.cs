using System.Threading.Tasks;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateCardMemberExistsAttribute : TypeFilterAttribute
{
  public ValidateCardMemberExistsAttribute():base(typeof
    (ValidateCardMemberExistsFilterImpl))
  {
  }
  private class ValidateCardMemberExistsFilterImpl : IAsyncActionFilter
  {
    private readonly ICardRepository _cardRepository;
    public ValidateCardMemberExistsFilterImpl(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("userId"))
      {
        var id = context.ActionArguments["userId"] as int?;
        if (id.HasValue)
        {
          if (await _cardRepository.GetCardMember(id.Value) == null)
          {
            context.ModelState.AddModelError(string.Empty, "The card member was not found.");
            context.Result = new NotFoundError(context.ModelState);
            return;
          }
        }
      }
      await next();
    }
  }
}