using Library.Api.Context;
using Library.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Services
{
    public sealed class BookService(AppDbContext context) : IBookService
    {
        private readonly AppDbContext _context = context;

        public async Task<bool> CreateAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken) > 0; 
        }
        public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default)
        {
            _context.Update(book);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
        public async Task<bool> DeleteAsync(string isbn, CancellationToken cancellationToken = default)
        {
            Book? book = await _context.Books.FindAsync(isbn, cancellationToken);
            if (book is null) return false;

            _context.Books.Remove(book);
            return await _context.SaveChangesAsync(cancellationToken) > 0;  
        }
        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Books.ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Book>> SearchByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(title))
                .ToListAsync(cancellationToken);
        }
        public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
        {
            return await _context.Books.FindAsync(isbn, cancellationToken);
        }
    }
}
