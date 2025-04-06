using Library.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
}