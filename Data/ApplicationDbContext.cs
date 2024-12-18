using Azure;
using BerberSite.Models;
using Microsoft.EntityFrameworkCore;

namespace BerberSite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
