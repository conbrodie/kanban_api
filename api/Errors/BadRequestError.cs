using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class BadRequestError : ObjectResult
{
    public BadRequestError(ModelStateDictionary modelState) 
        : base(new ValidationResultModel(modelState))
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
}