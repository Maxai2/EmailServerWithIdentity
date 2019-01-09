public interface IAccountService
{
    RegistrationResponse Registration(User user);
    LoginResponse LogIn(string login, string password);
    void LogOut(int id);
    User GetAccount(int id);
    LoginResponse UpdateToken(string refreshToken);
}