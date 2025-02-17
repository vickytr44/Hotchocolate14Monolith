using HotChocolateV14.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotChocolateV14;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Bill> Bills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var accountTypeConverter = new ValueConverter<AccountType, string>(
                v => v.ToString(),          
                v => (AccountType)Enum.Parse(typeof(AccountType), v)
            );

        var billMonthConverter = new ValueConverter<Month, string>(
                v => v.ToString(),
                v => (Month)Enum.Parse(typeof(Month), v)
            );

        var billStatusConverter = new ValueConverter<Status, string>(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v)
            );

        modelBuilder.Entity<Customer>().HasMany(c => c.Accounts).WithOne(a => a.Customer).HasForeignKey(a => a.CustomerId);

        modelBuilder.Entity<Customer>().HasMany(c => c.Bills).WithOne(b => b.Customer).HasForeignKey(b => b.CustomerId);

        modelBuilder.Entity<Account>().HasMany(a => a.Bills).WithOne(b => b.Account).HasForeignKey(b => b.AccountId);

        modelBuilder.Entity<Account>()
            .Property(a => a.Type)
            .HasConversion(accountTypeConverter);

        modelBuilder.Entity<Bill>()
            .Property(b => b.Month)
            .HasConversion(billMonthConverter);

        modelBuilder.Entity<Bill>()
            .Property(b => b.Status)
            .HasConversion(billStatusConverter);

        base.OnModelCreating(modelBuilder);
    }
}
