using System;

public class AccountToken
{
    public int AccountId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
}