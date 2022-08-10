using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOnline.Models
{
    public class BookOnlineDatabaseContext : DbContext
    {
        public BookOnlineDatabaseContext(DbContextOptions<BookOnlineDatabaseContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
