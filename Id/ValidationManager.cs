using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Owin_Auth.Utils;

namespace Owin_Auth.Id
{
    public class ValidationManager: IValidationManager
    {
        public async Task<string> RegisterUserForValidation(DataContext context,string username)
        {
            var ex = (await context.UserValidations.CountAsync()) == 0 ? null :  await context.UserValidations.FirstAsync(val => val.Username == username);
            if (ex != null)
            {
                throw new Exception("Validation with this id already exists");
            }
            UserValidation v = new UserValidation();
            v.Username = username;
            v.ValidUntil = DateTime.Now.AddDays(1.0);

            string random = Guid.NewGuid() + username;
            random = random.MD5();
            v.LongId = random;
            await context.UserValidations.AddAsync(v);
            await context.SaveChangesAsync();
            return random;
        }

        public async Task<bool> IsTrueValidation(DataContext context,string id)
        {
            var ifExsist = await context.UserValidations.FirstOrDefaultAsync(validation => validation.LongId == id);
            return ifExsist != null;
        }

        public async Task<string> GetIdForUsername(DataContext context, string username)
        {
            var val = await context.UserValidations.FirstOrDefaultAsync(validation => validation.Username == username);
            return val.LongId;
        }

        public async Task<UserValidation> GetValidationForUsername(DataContext context, string username)
        {
            var val = await context.UserValidations.FirstOrDefaultAsync(validation => validation.Username == username);
            return val;
        }
    }
}
