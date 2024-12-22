using BerberSite.Data;
using BerberSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BerberSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ------------------ Kullanıcı Yönetimi ------------------
        public IActionResult KullaniciYonetimi()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult YeniKullaniciEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult YeniKullaniciEkle(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("", "Bu e-posta ile kayıtlı kullanıcı zaten mevcut.");
                return View(model);
            }

            _context.Users.Add(model);
            _context.SaveChanges();

            // Eğer rol Employee ise Employee tablosuna da ekleyelim
            if (model.Role == Role.Employee)
            {
                var employee = new Employee
                {
                    UserId = model.Id,
                    Name = model.FirstName,
                    Surname = model.LastName,
                    Email = model.Email,
                    Password = model.Password
                };
                _context.Employees.Add(employee);
                _context.SaveChanges();
            }

            return RedirectToAction("KullaniciYonetimi");
        }

        [HttpGet]
        public IActionResult KullaniciDuzenle(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult KullaniciDuzenle(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.Find(model.Id);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            // Şifre alanına dokunmuyoruz (isteğe göre değişmediği varsayılmıştı)
            user.Role = model.Role;

            _context.SaveChanges();

            // Eğer user rolü Employee ise Employee tablosunu güncelle veya ekle
            if (user.Role == Role.Employee)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.UserId == user.Id);
                if (employee == null)
                {
                    // Yoksa yeni ekle
                    var newEmp = new Employee
                    {
                        UserId = user.Id,
                        Name = user.FirstName,
                        Surname = user.LastName,
                        Email = user.Email,
                        Password = user.Password
                    };
                    _context.Employees.Add(newEmp);
                }
                else
                {
                    // Varsa güncelle
                    employee.Name = user.FirstName;
                    employee.Surname = user.LastName;
                    employee.Email = user.Email;
                    // Password alanına dokunmuyoruz
                }
                _context.SaveChanges();
            }
            else
            {
                // Eğer rol Employee değilse, varsa Employee kaydını silelim
                var employee = _context.Employees.FirstOrDefault(e => e.UserId == user.Id);
                if (employee != null && user.Role != Role.Employee)
                {
                    _context.Employees.Remove(employee);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("KullaniciYonetimi");
        }

        [HttpPost]
        public IActionResult KullaniciSil(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            // Eğer employee ise Employee tablosundan da sil
            if (user.Role == Role.Employee)
            {
                var emp = _context.Employees.FirstOrDefault(e => e.UserId == user.Id);
                if (emp != null)
                {
                    _context.Employees.Remove(emp);
                }
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("KullaniciYonetimi");
        }
    }
}
