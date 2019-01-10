using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public interface IAccountService
{
    Task<IdentityResult> Registration(User account);
    Task<LoginResponse> LogIn(string login, string password);
    void LogOut(int id, string RefreshToken);
    Task<User> GetAccount(string id);
    Task<LoginResponse> UpdateToken(string refreshToken);
    string GetRefreshToken(int id);
}