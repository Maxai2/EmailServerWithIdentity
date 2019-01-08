public interface IAccountService
{
    Registration
    LoginResponse LogIn(string login, string password);
    void LogOut(int id);
    Account GetAccount(int id);
    void SendEmail();
    LoginResponse UpdateToken(string refreshToken);
}