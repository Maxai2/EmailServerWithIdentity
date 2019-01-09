using System;
using System.ComponentModel.DataAnnotations;

public class AccountToken
{
    public int Id { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }

    [Required]
    public virtual User User { get; set; }
}