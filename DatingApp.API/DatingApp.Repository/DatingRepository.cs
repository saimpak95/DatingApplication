using DatingApp.DomainModels;
using DatingApp.Repository.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Repository
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUserByID(int ID);

        Task<Photo> GetPhoto(int ID);
        Task<Photo> GetMainPhotoForUser(int UserID);
        Task<Like> GetLike(int userID, int recipientID);
        Task<Message> GetMessage(int MessageID);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userID, int recipientId);
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

        public async Task<Photo> GetMainPhotoForUser(int UserID)
        {
            return await db.Photos.Where(u => u.UserId == UserID).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int ID)
        {
            var photo = await db.Photos.FirstOrDefaultAsync(p=>p.Id==ID);
            return photo;
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

        //public async Task<IEnumerable<User>> GetUsers()
        //{
        //    var users = await db.Users.Include(p => p.Photos).ToListAsync();
        //    if (users != null)
        //    {
        //        return users;
        //    }
        //    return null;
        //}

        public async Task<bool> SaveAll()
        {
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = db.Users.Include(p => p.Photos).OrderByDescending(u=> u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserID);
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge != 18 || userParams.MaxAge != 90)
            {
                var minDob = DateTime.Now.AddYears(-userParams.MaxAge -1);
                var maxDob = DateTime.Now.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive); 
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await db.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }
        public async Task<Like> GetLike(int userID, int recipientID)
        {
            return await db.Likes.FirstOrDefaultAsync(u => u.LikerId == userID && u.LikeeId == recipientID);
        }

        public async Task<Message> GetMessage(int MessageID)
        {
            return await db.Messages.FirstOrDefaultAsync(m => m.Id == MessageID);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var message = db.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos).Include(u => u.Recipient).ThenInclude(p=>p.Photos).AsQueryable();
            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    message = message.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    message = message.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                case "Unread":
                    message = message.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                    break;
                default:
                    break;
            }
            message = message.OrderByDescending(d=>d.MessageSent);
            return await PagedList<Message>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userID, int recipientId)
        {
            var message = await db.Messages.Include(u => u.Sender)
                                     .ThenInclude(p => p.Photos)
                                     .Include(u => u.Recipient)
                                     .ThenInclude(p => p.Photos)
                                     .Where(m=>m.RecipientId == userID && m.RecipientDeleted == false && m.SenderId == recipientId || m.RecipientId == recipientId && m.SenderId == userID && m.SenderDeleted==false)
                                     .OrderBy(m=>m.MessageSent)
                                     .ToListAsync();
            return message;

        }
    }
}
