using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Copies { get; set; }
        public int AvaibleCopies { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string? WhoDeleted { get; set; }

        public virtual ICollection<BookLoan> BooksLoans { get; set; }
    }
}
