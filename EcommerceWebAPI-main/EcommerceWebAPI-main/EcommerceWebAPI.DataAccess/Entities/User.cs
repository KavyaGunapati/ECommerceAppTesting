
using Microsoft.AspNetCore.Identity;

namespace ECommerceWebAPI.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Relationship: One User → Many Orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
