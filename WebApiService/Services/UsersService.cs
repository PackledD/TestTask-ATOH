using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace WebApiService.Services
{
    public class UsersService
    {
        private UsersContext _ctx;

        public UsersService(UsersContext ctx) => _ctx = ctx;

        private ValueTask<User?> _GetUserAsync(Guid guid)
        {
            return _ctx.Users.FindAsync(guid);
        }

        private Task<User?> _GetUserAsync(String login)
        {
            return _ctx.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> IsActive(Guid guid)
        {
            var user = await _GetUserAsync(guid);
            return user != null && !user.RevokedOn.HasValue;
        }

        public async Task<bool> IsActive(string login)
        {
            var user = await _GetUserAsync(login);
            return user != null && !user.RevokedOn.HasValue;
        }

        public async Task<User?> AddUserAsync(User newUser, string createBy)
        {
            try
            {
                newUser.CreatedOn = DateTime.UtcNow;
                newUser.CreatedBy = createBy;
                await _ctx.Users.AddAsync(newUser);
                await _ctx.SaveChangesAsync();
                return newUser;
            }
            catch
            {
                return null;
            }
        }

        public async Task<User?> UpdateUserDataAsync(Guid guid, string updateBy, string? name = null, int? gender = null, DateTime? birthday = null)
        {
            var user = await _GetUserAsync(guid);
            if (user != null)
            {
                int c = 0;
                if (name != null)
                {
                    c++;
                    user.Name = name;
                }
                if (gender != null)
                {
                    c++;
                    user.Gender = gender.Value;
                }
                if (birthday != null)
                {
                    c++;
                    user.Birthday = birthday;
                }
                if (c > 0)
                {
                    user.ModifiedOn = DateTime.UtcNow;
                    user.ModifiedBy = updateBy;
                    try
                    {
                        _ctx.Users.Update(user);
                        await _ctx.SaveChangesAsync();
                    }
                    catch
                    {
                        user = null;
                    }
                }
            }
            return user;
        }

        public async Task<User?> UpdatePasswordAsync(Guid guid, string password, string updateBy)
        {
            var user = await _GetUserAsync(guid);
            if (user != null)
            {
                user.Password = password;
                user.ModifiedBy = updateBy;
                user.ModifiedOn = DateTime.UtcNow;
                try
                {
                    _ctx.Users.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }

        public async Task<User?> UpdateLoginAsync(Guid guid, string login, string updateBy)
        {
            var user = await _GetUserAsync(guid);
            if (user != null)
            {
                user.Login = login;
                user.ModifiedBy = updateBy;
                user.ModifiedOn = DateTime.UtcNow;
                try
                {
                    _ctx.Users.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }

        public async Task<ICollection<User>> GetActiveUsersAsync()
        {
            return await _ctx.Users.Where(u => !u.RevokedOn.HasValue).OrderBy(u => u.CreatedOn).ToListAsync();
        }

        public async Task<User?> GetUserAsync(string login)
        {
            return await _GetUserAsync(login);
        }

        public async Task<User?> GetUserWithPasswordAsync(string login, string password)
        {
            var user = await _GetUserAsync(login);
            return user?.Password == password ? user : null;
        }

        public async Task<ICollection<User>> GetElderUsersAsync(int age)
        {
            return await _ctx.Users.Where(u => u.Age() > age).ToListAsync();
        }

        public async Task<User?> DeleteUserHardAsync(string login)
        {
            var user = await _GetUserAsync(login);
            if (user != null)
            {
                try
                {
                    _ctx.Users.Remove(user);
                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }

        public async Task<User?> DeleteUserSoftAsync(string login, string deleteBy)
        {
            var user = await _GetUserAsync(login);
            if (user != null)
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = deleteBy;
                try
                {
                    _ctx.Users.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }

        public async Task<User?> RestoreUserAsync(string login)
        {
            var user = await _GetUserAsync(login);
            if (user != null)
            {
                user.RevokedOn = null;
                user.RevokedBy = null;
                try
                {
                    _ctx.Users.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }
    }
}
