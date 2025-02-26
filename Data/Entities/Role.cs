using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public  class Role : IdentityRole<Guid>

    {
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public string Description { get; set; }
    }
}
