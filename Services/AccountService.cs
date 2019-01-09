using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class AccountService : IAccountService
{
    private List<User> users;
    private List<AccountToken> accountTokens;
    private AuthOptions authOptions;

    private UserManager<User> userManager;
    private SignInManager<User> signInManager;

    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    // public AccountService(IOptions<AuthOptions> options)
    // {
    //     this.accounts = new List<Account>()
    //     {
    //         new Account() {Login = "user1", Password = "1111"},
    //         new Account() {Login = "user2", Password = "1111"}
    //     };

    //     accountTokens = new List<AccountToken>();
    //     authOptions = options.Value;
    // }

    public async RegistrationResponse Registration(User account)
    {
        var acc = Authentication(account);

        User user = new User()
        {
            Email = account.Email,
            UserName = account.UserName
        };

        IdentityResult res = await userManager.CreateAsync(user, account.PasswordHash);

        if (res.Succeeded)
        {
            await signInManager.SignInAsync(user, true);
        }

        var user = new RegistrationResponse()
        {
            AccessToken = acc.AccessToken,
            Email = account.Email,
            Login = acc.Login,
            Password = account.Password,
            RefreshToken = acc.RefreshToken
        };

        return user;
    }

    public User GetAccount(int id)
    {
       return users.Find(a => Int32.Parse(a.Id) == id);
    }

    public async Task<ActionResult<LoginResponse>> LogIn(string login, string password)
    {
        var user = await userManager.FindByNameAsync(login);

        var res = await signInManager.PasswordSignInAsync(user, password, false, false);

        if (res.Succeeded)
        {
            var acc = Authentication(user);
            return acc;
        }
        else
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Users not found")
        }
    }

    public async void LogOut(int id)
    {
       accountTokens.RemoveAll(ac => ac.Id == id);
       await signInManager.SignOutAsync();
    }

    public LoginResponse UpdateToken(string refreshToken)
    {
        AccountToken accountToken = accountTokens.Find(at => at.RefreshToken == refreshToken);

        if (accountToken == null)
            return null;

        if (accountToken.Expires <= DateTime.Now)
            return null;

        User user = users.Find(a => Int32.Parse(a.Id) == accountToken.Id);

        if (user == null)
            return null;

        return Authentication(user);
    }

    private LoginResponse Authentication(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim("id", user.Id.ToString())
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
            Login = user.UserName,
            RefreshToken = Guid.NewGuid().ToString()
        };

        accountTokens.RemoveAll(at => at.Id == Int32.Parse(user.Id));

        accountTokens.Add(new AccountToken()
        {
            Id = Int32.Parse(user.Id),
            Expires = DateTime.Now.AddMinutes(authOptions.RefreshLifetime),
            RefreshToken = resp.RefreshToken
        });

        return resp;
    }
}
