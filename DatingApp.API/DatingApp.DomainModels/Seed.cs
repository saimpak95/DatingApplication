using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DatingApp.DomainModels
{
   public class Seed
    {
        public static void SeedUsers(DataContext db) {
            if (!db.Users.Any())
            {
                var path = "../DatingApp.DomainModels/SeedData.json";
                var userData = System.IO.File.ReadAllText(path);
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordSalt, passwordHash;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    db.Users.Add(user);
                }
                db.SaveChanges();
            }
                    
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {                  
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }


        }
    }
}
