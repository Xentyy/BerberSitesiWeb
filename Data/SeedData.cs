using BerberSite.Models;
using Microsoft.EntityFrameworkCore;

namespace BerberSite.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                if (!context.Users.Any(u => u.Email == "g211210034@sakarya.edu.tr"))
                {
                    var admin = new User
                    {
                        FirstName = "Melih",
                        LastName = "Şengün",
                        Email = "g211210034@sakarya.edu.tr",
                        PhoneNumber = "05532871054",
                        Password = "sau",
                        Role = Role.Admin
                    };
                    context.Users.Add(admin);
                    context.SaveChanges();
                }

                if (!context.Users.Any(u => u.Email == "g211210038@sakarya.edu.tr"))
                {
                    var admin = new User
                    {
                        FirstName = "Fatih",
                        LastName = "Uçar",
                        Email = "g211210038@sakarya.edu.tr",
                        PhoneNumber = "05314363494",
                        Password = "sau",
                        Role = Role.Admin
                    };
                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }
        }
    }
}
