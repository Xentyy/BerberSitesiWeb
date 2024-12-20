using BerberSite.Data;
using BerberSite.Models;
using BerberSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BerberSite.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Bu e-posta adresi ile kayıtlı bir kullanıcı zaten mevcut.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = model.Password,
                Role = Role.Customer // Her zaman Customer
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
