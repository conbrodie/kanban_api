using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ValidationResultModel
{
    public string Message { get; } 
    public int StatusCode { get; } 
    public List<ValidationError> Errors { get; }
    public ValidationResultModel(ModelStateDictionary modelState)
    {
        StatusCode = 400;
        Message = "Validation Failed";
        Errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
    }
}