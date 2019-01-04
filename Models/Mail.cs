using System;
using System.ComponentModel.DataAnnotations;

public class Mail
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string MailText { get; set; }
    [Required]
    public DateTime MailTime { get; set; }

    [Required]
    public virtual User User { get; set; }
}