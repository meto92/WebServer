using System.Linq;
using System.Security.Cryptography;
using System.Text;

using IRunes.Services.Contracts;

namespace IRunes.Services
{
    public class HashService : IHashService
    {
        private const string Salt = "!@#$%()*&^_-+=<?>";

        public string Hash(string stringToHash)
        {
            SHA256 sha = SHA256.Create();

            using (sha)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(stringToHash + Salt);
                byte[] hashedBytes = sha.ComputeHash(bytes);

                string hash = string.Join(
                    string.Empty,
                    hashedBytes.Select(num => $"{num:x}"));

                return hash;
            }
        }
    }
}