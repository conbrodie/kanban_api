using System;
using System.Net;
using Newtonsoft.Json.Linq;

public class HttpStatusCodeException : Exception
{
    private readonly HttpStatusCode statusCode;
    private readonly JObject errorObject;

    public HttpStatusCode StatusCode { get; set; }
    public string ContentType { get; set; } = @"text/plain";

    public HttpStatusCodeException(HttpStatusCode statusCode)
    {
        this.StatusCode = statusCode;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode, string message) 
        : base(message)
    {
        this.StatusCode = statusCode;
    }

    public HttpStatusCodeException(HttpStatusCode statusCode, Exception inner) 
        : this(statusCode, inner.ToString()) { }

    public HttpStatusCodeException(HttpStatusCode statusCode, JObject errorObject) 
        : this(statusCode, errorObject.ToString())
    {
        this.ContentType = @"application/json";
        this.statusCode = statusCode;
        this.errorObject = errorObject;
    }

}