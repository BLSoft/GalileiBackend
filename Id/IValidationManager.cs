using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Owin_Auth.Utils;

namespace Owin_Auth.Id
{
    public interface IValidationManager
    {
        Task<string> RegisterUserForValidation(DataContext context, string username);
        Task<bool>   IsTrueValidation(DataContext context,string username);
    }
}
