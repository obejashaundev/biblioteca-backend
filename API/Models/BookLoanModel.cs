namespace API.Models
{
    public class BookLoanModel
    {
        public int BookId { get; set; }
        public int PersonId { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
