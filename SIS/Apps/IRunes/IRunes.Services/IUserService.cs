using IRunes.Models;

namespace IRunes.Services
{
    public interface IUserService
    {
        bool Exists(string username);

        bool Create(string username, string password, string email);

        User Get(string username);

        User Get(string usernameOrEmail, string password);
    }
}