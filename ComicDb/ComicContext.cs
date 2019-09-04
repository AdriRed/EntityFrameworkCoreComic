using System;
using JetBrains.Annotations;
using ComicDb.Classes;
using Microsoft.EntityFrameworkCore;

namespace ComicDb
{
    public class ComicContext : DbContext
    {
        public ComicContext()
        {
            db = this;
        }
        public static ComicContext db;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=192.168.0.22;Database=ComicsDB;User Id=sa;Password=pepe1234!;");
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Function> Functions { get; set; }
    }
}
