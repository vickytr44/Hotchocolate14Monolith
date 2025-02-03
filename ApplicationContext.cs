using HotChocolateV14.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotChocolateV14;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var accountTypeConverter = new ValueConverter<AccountType, string>(
                v => v.ToString(),          
                v => (AccountType)Enum.Parse(typeof(AccountType), v)
            );

        modelBuilder.Entity<Customer>().HasMany(c => c.Accounts).WithOne(a => a.Customer).HasForeignKey(a => a.CustomerId);

        modelBuilder.Entity<Account>()
            .Property(a => a.Type)
            .HasConversion(accountTypeConverter);

        base.OnModelCreating(modelBuilder);
    }
}
