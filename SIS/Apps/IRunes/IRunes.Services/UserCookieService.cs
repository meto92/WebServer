using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IRunes.Services
{
    public class UserCookieService : IUserCookieService
    {
        private const string PasswordHash = "P@$Sw0rD";
        private const string SaltKey = "S@LT&KEY";
        private const string VIKey = "@1B2c3D4e5F6g7H8";

        public string GetEncryptedUsername(string username)
        {
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

            var symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };

            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherUsernameBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(usernameBytes, 0, usernameBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherUsernameBytes = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(cipherUsernameBytes);
        }

        public string GetDecryptedUsername(string encryptedUsername)
        {
            byte[] cipherUsernameBytes = Convert.FromBase64String(encryptedUsername);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

            var symmetricKey = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherUsernameBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] usernameBytes = new byte[cipherUsernameBytes.Length];

            int decryptedByteCount = cryptoStream
                .Read(usernameBytes, 0, usernameBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.UTF8.GetString(usernameBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}