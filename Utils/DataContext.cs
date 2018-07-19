using Microsoft.EntityFrameworkCore;
using Owin_Auth.Id;

namespace Owin_Auth.Utils
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserValidation> UserValidations { get; set; }
    }
}