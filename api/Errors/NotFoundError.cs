using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class NotFoundError : ObjectResult
{
    public NotFoundError(ModelStateDictionary modelState) 
        : base(new NotFoundErrorModel(modelState))
    {
        StatusCode = StatusCodes.Status404NotFound;
    }
}