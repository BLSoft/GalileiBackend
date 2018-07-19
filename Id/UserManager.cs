using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Owin_Auth.Utils;

namespace Owin_Auth.Id
{
    public class UserManager : IUserManager
    {
        

        public async Task<UserRegistrationResult> RegisterNewUser(DataContext context,string username, string password, string email)
        {
            try
            {

                var ex = (await context.Users.CountAsync()) == 0 ? null :  await context.Users.FirstAsync(user => user.Username == username);
                if (ex != null)
                {
                    return UserRegistrationResult.EXISTS;
                }
                else
                {
                    User u = new User();
                    u.Email = email;
                    u.Username = username;
                    u.IsLocked = false;
                    u.IsVerified = false;
                    u.Role = 2;

                    PasswordHasher<User> ph = new PasswordHasher<User>();
                    string hash = ph.HashPassword(u, password);
                    u.PasswordHash = hash;

                    await context.Users.AddAsync(u);
                    context.SaveChanges();
                    return UserRegistrationResult.SUCCESS;
                }
            }
            catch (Exception e)
            {
                throw e;
                return UserRegistrationResult.ERROR;
            }
        }

        public async Task<(LoginResult status, User user)> LoginUser(DataContext context,string requestUsername, string requestPassword)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(user1 => user1.Username == requestUsername);
                if (user != null)
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    var result  = hasher.VerifyHashedPassword(user, user.PasswordHash, requestPassword);
                    if (result == PasswordVerificationResult.Success)
                    {
                        if (user.IsVerified)
                        {
                            if (!user.IsLocked)
                            {
                                return (LoginResult.SUCCESS, user);
                            }
                            else
                            {
                                return (LoginResult.LOCKED, null);
                            }
                        }
                        else
                        {
                            return (LoginResult.NOT_ACTIVATED, null);
                        }
                    }
                    else
                    {
                        return (LoginResult.BAD_PASSWORD, null);
                    }
                }
                else
                {
                    return (LoginResult.BAD_USER, null);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<User> GetUserById(DataContext context, int userId)
        {
            var usr = await context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
            return usr;
        }
    }
}
