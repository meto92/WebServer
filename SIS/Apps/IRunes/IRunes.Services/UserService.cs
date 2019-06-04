using System;
using System.Linq;

using IRunes.Data;
using IRunes.Models;

namespace IRunes.Services
{
    public class UserService : IUserService
    {
        private readonly RunesDbContext db;

        public UserService()
            => this.db = new RunesDbContext();

        public bool Create(string username, string password, string email)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Email = email
            };

            try
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Exists(string username)
            => this.db.Users.Any(u => u.Username == username);

        public User Get(string usernameOrEmail, string password)
            => this.db.Users
                .FirstOrDefault(u => u.Password == password
                    && (u.Username == usernameOrEmail || u.Email == usernameOrEmail));

        public User Get(string username)
            => this.db.Users.FirstOrDefault(u => u.Username == username);
    }
}