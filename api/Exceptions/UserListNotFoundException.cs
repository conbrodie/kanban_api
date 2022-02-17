using System;

public class ListNotFoundException : Exception 
{
    public ListNotFoundException() {

    }

    public ListNotFoundException(string message)
        : base(message)
    {
    }

    public ListNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
