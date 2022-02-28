using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class InternalServerError : ObjectResult
{
    public InternalServerError() 
        : base(new InternalServerErrorModel())
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}