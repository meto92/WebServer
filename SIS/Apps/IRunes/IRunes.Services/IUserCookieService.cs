namespace IRunes.Services
{
    public interface IUserCookieService
    {
        string GetEncryptedUsername(string username);

        string GetDecryptedUsername(string encryptedUsername);
    }
}