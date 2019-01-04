using System.ComponentModel.DataAnnotations;

public class RegistrationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Login { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [Display(Name = "Repeat")]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string RepeatPassword { get; set; }
}