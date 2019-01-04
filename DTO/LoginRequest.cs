using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required]
    public string Login { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}