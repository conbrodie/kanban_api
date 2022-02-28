using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class InternalServerErrorModel
{
    public string Message { get; } 
    public int StatusCode { get; } 
    public InternalServerErrorModel()
    {
        StatusCode = 500;
        Message = "Internal Server Error";
    }
}