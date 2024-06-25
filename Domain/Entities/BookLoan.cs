using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BookLoan
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int BookId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? RealReturnDate { get; set; }
        public Person Person { get; set; }
        public Book Book { get; set; }
    }
}
