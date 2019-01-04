using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class AccountService : IAccountService
{
    private List<Account> accounts;
    private List<AccountToken> accountTokens;
    private AuthOptions authOptions;

    public AccountService(IOptions<AuthOptions> options)
    {
        // this.accounts = 
    }
    public Account GetAccount(int id)
    {
        throw new System.NotImplementedException();
    }

    public LoginResponse LogIn(string login, string password)
    {
        throw new System.NotImplementedException();
    }

    public void LogOut(int id)
    {
        throw new System.NotImplementedException();
    }

    public void SendEmail()
    {
        throw new System.NotImplementedException();
    }

    public LoginResponse UpdateToken(string refreshToken)
    {
        throw new System.NotImplementedException();
    }

    private LoginResponse Authentication(Account account)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, account.Login),
            new Claim("id", account.Id.ToString())
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        JwtSecurityToken token = new JwtSecurityToken
            (
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                claims: claimsIdentity.Claims,
                expires: DateTime.Now.AddMinutes(authOptions.AccessLifetime),
                signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

        string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        LoginResponse resp = new LoginResponse()
        {
            AccessToken = tokenStr,
            Login = account.Login,
            RefreshToken = Guid.NewGuid().ToString()
        };

        accountTokens.RemoveAll(at => at.AccountId == account.Id);

        accountTokens.Add(new AccountToken()
        {
            AccountId = account.Id,
            Expires = DateTime.Now.AddMinutes(authOptions.RefreshLifetime),
            RefreshToken = resp.RefreshToken
        });

        return resp;
    }
}
