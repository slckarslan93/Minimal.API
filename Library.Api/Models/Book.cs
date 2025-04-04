using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
    public class Book
    {
        [Key]
        public string Isbn { get; set; } =default!;
        public string Title { get; set; } = default!;
        public string ShortDescription { get; set; } = default!;
        public int PageCount { get; set; }
        public DateTime PublishDate { get; set; }

    }
}
