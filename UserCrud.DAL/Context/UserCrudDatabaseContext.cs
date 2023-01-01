using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCrud.Domain.Entities;

namespace UserCrud.DAL.Context
{
    public class UserCrudDatabaseContext : DbContext
    {
        public UserCrudDatabaseContext(DbContextOptions<UserCrudDatabaseContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UsersRole> UsersRoles { get; set; }
    }
}
