using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotChocolateV14.Entities
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int IdentityNumber { get; set; }

        public int Age {  get; set; }

        [UseFiltering]
        [UseSorting]
        public virtual ICollection<Account> Accounts { get; set; }

        [UseFiltering]
        [UseSorting]
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
