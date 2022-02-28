using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Models
{
    public class AppUser:IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public DateTime Birthday { get; set; }
    }
}
