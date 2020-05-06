using Microsoft.EntityFrameworkCore;
using Test.model;

namespace Test.dataaccess
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}