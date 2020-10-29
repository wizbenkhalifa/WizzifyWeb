using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WizzifyWeb.Models;

namespace WizzifyWeb.Data
{
    public class WizzifyContext : DbContext
    {
        public DbSet<Account> Account { get; set; }

        public WizzifyContext(DbContextOptions<WizzifyContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(account => account.username);
            base.OnModelCreating(modelBuilder);
        }
        
    }
}
