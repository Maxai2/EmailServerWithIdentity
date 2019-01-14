using System;
using System.Linq;
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
    private EmailDbContext context;
    private AuthOptions authOptions;
    private UserManager<User> userManager;
    private SignInManager<User> signInManager;

    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, EmailDbContext context, IOptions<AuthOptions> options)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.context = context;
        this.authOptions = options.Value;
    }

    public async Task<object> Registration(string email, string login, string password)
    {
        User user = new User()
        {
            Email = email,
            UserName = login,
            SendMailCount = 0,
            DeleiveredMailCount = 0,
            TodayMailCount = 0,
            TodayMailCountLeft = 100,
            DeliveredMailToday = 0
        };

        var acc = Authentication(user);

        IdentityResult res = await userManager.CreateAsync(user, password);

        if (res.Succeeded)
            return acc;

        return res;
    }

    public Task<User> GetAccount(string id)
    {
        return userManager.FindByIdAsync(id);
    }

    public async Task<LoginResponse> LogIn(string login, string password)
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
            return null;
        }
    }

    public async void LogOut(int id, string RefreshToken)
    {
        var tok = context.Tokens.Find(id, RefreshToken);

        context.Tokens.Remove(tok);
        context.SaveChanges();
        await signInManager.SignOutAsync();
    }

    public async Task<LoginResponse> UpdateToken(string refreshToken)
    {
        AccountToken accountToken = context.Tokens.Find(refreshToken);

        if (accountToken == null)
            return null;

        if (accountToken.Expires <= DateTime.Now)
            return null;

        User user = await userManager.FindByIdAsync(accountToken.Id.ToString());

        if (user == null)
            return null;

        return Authentication(user);
    }
    public string GetRefreshToken(int id)
    {
        return context.Tokens.Find(id).RefreshToken;
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

        var tok = context.Tokens.Where(t => t.User.Id == user.Id).FirstOrDefault();

        if (tok != null)
        {
            context.Tokens.Remove(tok);
        }

        context.Tokens.Add(new AccountToken()
        {
            Expires = DateTime.Now.AddMinutes(authOptions.RefreshLifetime),
            RefreshToken = resp.RefreshToken,
            User = user
        });

        context.SaveChanges();

        return resp;
    }
}
