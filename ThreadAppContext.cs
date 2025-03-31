using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp40
{
    public class ThreadAppContext : DbContext
    {
        public DbSet<Banan> banan { set; get; }
        public ThreadAppContext()
        {

        }
        public ThreadAppContext(DbContextOptions<ThreadAppContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=ep-square-bar-a21rbkc4-pooler.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_Nd9aqQl4XLiZ");
        }
    }
}
