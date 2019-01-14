using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public int SendMailCount { get; set; }
    public int DeleiveredMailCount { get; set; }
    public int TodayMailCount { get; set; }
    public int TodayMailCountLeft { get; set; }
    public int DeliveredMailToday { get; set; }
    public virtual List<Mail> Mails { get; set; } = new List<Mail>();
}