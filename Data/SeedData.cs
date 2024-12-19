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
                // Admin kullanıcı var mı?
                if (!context.Users.Any(u => u.Email == "melih.sengun@sakarya.edu.tr"))
                {
                    var admin = new User
                    {
                        FirstName = "Melih",
                        LastName = "Şengün",
                        Email = "melih.sengun@sakarya.edu.tr",
                        PhoneNumber = "05532871054",
                        Password = "sau", // gerçek uygulamada hash
                        Role = Role.Admin
                    };
                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }
        }
    }
}
