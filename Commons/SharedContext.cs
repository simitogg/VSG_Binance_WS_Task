using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    public class SharedContext : DbContext
    {
        public DbSet<SymbolRecord> SymbolRecords { get; set; }

        public SharedContext(DbContextOptions<SharedContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SymbolRecord>().ToTable("SymbolRecords");
        }
    }
}
