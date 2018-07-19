using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Owin_Auth.Utils;

namespace Owin_Auth.Id
{
    public interface IUserManager
    {
        Task<UserRegistrationResult> RegisterNewUser(DataContext context,string username, string password, string email);

        Task<(LoginResult status, User user)> LoginUser(DataContext context,string requestUsername, string requestPassword);
    }

    public enum UserRegistrationResult
    {
        SUCCESS,EXISTS,ERROR
    }

    public enum LoginResult
    {
        BAD_USER,BAD_PASSWORD,NOT_ACTIVATED,LOCKED,ERROR,SUCCESS
    }
}
