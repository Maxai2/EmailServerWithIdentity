using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class CustomException : ApplicationException
{
    public CustomException(IEnumerable<IdentityError> listErrors)
    {
        ListErrors = listErrors;
    }

    public IEnumerable<IdentityError> ListErrors { get; set; }
}