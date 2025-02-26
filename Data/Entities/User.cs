using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User: IdentityUser<Guid>
    {
        public DateTime Dob { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Order> Orders { get; set; }

        public List<Transaction> Transactions { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; }
        public virtual ICollection<UserLogin> UserLogins { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
    }
}
