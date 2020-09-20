using DatingApp.DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DatingApp.Repository
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);

        Task<User> Login(string username, string password);

        Task<bool> UserExists(string username);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext db;

        public AuthRepository(DataContext db)
        {
            this.db = db;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(temp => temp.UserName == username);
            if (user == null)
                return null;
            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;
            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await db.Users.AnyAsync(x => x.UserName == username))
                return true;
            else
                return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
    }
}