using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public virtual List<Mail> Mails { get; set; }
}