using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using ComicStoreDb.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace ComicStoreDb
{
    public class ComicsContext : DbContext
    {
        public static ComicsContext db;

        public ComicsContext() : base()
        {
            db = this;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=ComicsDB;Trusted_Connection=True;");
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Function> Functions { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //}
    }
}
