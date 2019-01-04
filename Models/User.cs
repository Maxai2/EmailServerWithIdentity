using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public List<Mail> Mails { get; set; }
}