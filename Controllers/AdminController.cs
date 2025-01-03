﻿using BerberSite.Data;
using BerberSite.Models;
using BerberSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BerberSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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
            user.Role = model.Role;

            _context.SaveChanges();
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
                    // Password değiştirmiyoruz
                }
                _context.SaveChanges();
            }
            else
            {
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

        // ------------------ Personel Yönetimi ------------------
        public IActionResult PersonelYonetimi()
        {
            var employees = _context.Employees.Include(e => e.User).ToList();
            return View(employees);
        }

        [HttpGet]
        public IActionResult PersonelDuzenle(int id)
        {
            var employee = _context.Employees
                .Include(e => e.User)
                .Include(e => e.EmployeeOperations).ThenInclude(eo => eo.Operation)
                .Include(e => e.WorkingHours)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null) return NotFound();

            var allOperations = _context.Operations.ToList();

            var viewModel = new PersonelDuzenleViewModel
            {
                Employee = employee,
                AllOperations = allOperations,
                NewWorkingHour = new WorkingHour { EmployeeId = employee.Id }
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult PersonelOperationEkle(int employeeId, int operationId)
        {
            var employee = _context.Employees.Find(employeeId);
            var operation = _context.Operations.Find(operationId);
            if (employee == null || operation == null)
                return NotFound();

            bool alreadyExists = _context.EmployeeOperations.Any(eo => eo.EmployeeId == employeeId && eo.OperationId == operationId);
            if (!alreadyExists)
            {
                _context.EmployeeOperations.Add(new EmployeeOperation { EmployeeId = employeeId, OperationId = operationId });
                _context.SaveChanges();
            }

            return RedirectToAction("PersonelDuzenle", new { id = employeeId });
        }

        [HttpPost]
        public IActionResult PersonelOperationSil(int employeeId, int operationId)
        {
            var eo = _context.EmployeeOperations.FirstOrDefault(e => e.EmployeeId == employeeId && e.OperationId == operationId);
            if (eo == null) return NotFound();

            _context.EmployeeOperations.Remove(eo);
            _context.SaveChanges();

            return RedirectToAction("PersonelDuzenle", new { id = employeeId });
        }

        [HttpPost]
        public IActionResult CalismaSaatiEkle(WorkingHour model)
        {
            if (model.StartTime >= model.EndTime)
            {
                TempData["CalismaSaatiHata"] = "Başlangıç saati bitiş saatinden önce olmalıdır.";
                return RedirectToAction("PersonelDuzenle", new { id = model.EmployeeId });
            }

            _context.WorkingHours.Add(model);
            _context.SaveChanges();
            return RedirectToAction("PersonelDuzenle", new { id = model.EmployeeId });
        }

        [HttpPost]
        public IActionResult CalismaSaatiSil(int workingHourId)
        {
            var wh = _context.WorkingHours.Find(workingHourId);
            if (wh == null) return NotFound();

            int empId = wh.EmployeeId;
            _context.WorkingHours.Remove(wh);
            _context.SaveChanges();

            return RedirectToAction("PersonelDuzenle", new { id = empId });
        }

        // ------------------ Randevu Yönetimi ------------------
        public IActionResult RandevuYonetimi()
        {
            var appointments = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Operation)
                .ToList();
            return View(appointments);
        }

        // ------------------ İşlem Yönetimi ------------------
        public IActionResult IslemYonetimi()
        {
            var operations = _context.Operations.ToList();
            return View(operations);
        }

        [HttpGet]
        public IActionResult YeniIslemEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult YeniIslemEkle(Operation model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Operations.Add(model);
            _context.SaveChanges();
            return RedirectToAction("IslemYonetimi");
        }

        [HttpGet]
        public IActionResult IslemDuzenle(int id)
        {
            var op = _context.Operations.Find(id);
            if (op == null) return NotFound();
            return View(op);
        }

        [HttpPost]
        public IActionResult IslemDuzenle(Operation model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var op = _context.Operations.Find(model.Id);
            if (op == null) return NotFound();

            op.OperationName = model.OperationName;
            op.Price = model.Price;
            op.Duration = model.Duration;
            _context.SaveChanges();

            return RedirectToAction("IslemYonetimi");
        }

        [HttpPost]
        public IActionResult IslemSil(int id)
        {
            var op = _context.Operations.Find(id);
            if (op == null) return NotFound();

            _context.Operations.Remove(op);
            _context.SaveChanges();

            return RedirectToAction("IslemYonetimi");
        }


        // ------------------ Haftanın Personeli Bulma ------------------

        [HttpGet]
        public async Task<IActionResult> HaftaninPersoneli()
        {
            string apiUrl = "https://localhost:7007/api/reportapi/haftaninpersoneli";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Message = $"API isteğinde hata oluştu: {response.StatusCode}";
                    return View();
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(jsonString);

                ViewBag.Message = jObject["message"]?.ToString();
                ViewBag.EmployeeName = jObject["employeeName"]?.ToString();
                ViewBag.TotalEarnings = jObject["totalEarnings"]?.ToString();
            }

            return View();
        }

    }
}
