using System;
using System.ComponentModel.DataAnnotations;

public class AccountToken
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string RefreshToken { get; set; }
    [Required]
    public DateTime Expires { get; set; }

    [Required]
    public virtual User User { get; set; }
}