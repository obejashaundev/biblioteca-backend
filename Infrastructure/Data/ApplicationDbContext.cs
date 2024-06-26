using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BookLoan> BookLoans { get; set; }
        public DbSet<Person> Persons { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookLoan>()
            .HasOne(bookLoan => bookLoan.Person)
            .WithMany(person => person.BooksLoans)
            .HasForeignKey(bookLoan => bookLoan.PersonId);

            builder.Entity<BookLoan>()
                .HasOne(bookLoan => bookLoan.Book)
                .WithMany(book => book.BooksLoans)
                .HasForeignKey(bookLoan => bookLoan.BookId);

            base.OnModelCreating(builder);
        }
    }
}
