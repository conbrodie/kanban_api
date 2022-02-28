using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class NotFoundErrorModel
{
    public string Message { get; } 
    public int StatusCode { get; } 
    public List<ValidationError> Errors { get; }
    public NotFoundErrorModel(ModelStateDictionary modelState)
    {
        StatusCode = 404;
        Message = "Not Found";
        Errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
    }
}