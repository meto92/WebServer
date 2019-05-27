namespace IRunes.Services.Contracts
{
    public interface IUserCookieService
    {
        string GetEncryptedUsername(string username);

        string GetDecryptedUsername(string encryptedUsername);
    }
}