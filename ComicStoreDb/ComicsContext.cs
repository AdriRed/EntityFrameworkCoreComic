#define HOUSE
//#undef HOUSE

using ComicStoreDb.Classes;
using Microsoft.EntityFrameworkCore;

namespace ComicStoreDb
{
    public class ComicsContext : DbContext
    {
        public static ComicsContext Instance { get; private set; }

        public ComicsContext() : base()
        {
            Instance = this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if (HOUSE)
            optionsBuilder.UseSqlServer("Server = 192.168.0.22; Database = ComicsDB; User Id = sa; Password = pepe1234! ");
#else
            optionsBuilder.UseSqlServer("Server=localhost;Database=ComicsDB;Trusted_Connection=True;");
#endif
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Function> Functions { get; set; }
    }
}