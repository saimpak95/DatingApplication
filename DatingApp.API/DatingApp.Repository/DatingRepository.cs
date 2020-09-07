using DatingApp.DomainModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Repository
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserByID(int ID);
    }
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext db;

        public DatingRepository(DataContext db)
        {
            this.db = db;
        }
        public void Add<T>(T entity) where T : class
        {
            db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            db.Remove(entity);
        }

        public async Task<User> GetUserByID(int ID)
        {
            var user = await db.Users.Include(temp => temp.Photos).FirstOrDefaultAsync(f => f.Id == ID);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await db.Users.Include(p => p.Photos).ToListAsync();
            if (users != null)
            {
                return users;
            }
            return null;
        }

        public async Task<bool> SaveAll()
        {
            return await db.SaveChangesAsync() > 0;
        }
    }
}
