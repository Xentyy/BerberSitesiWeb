using BerberSite.Data;
using BerberSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BerberSite.Controllers
{
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }
        // 1. Adım: Operasyon seç
        [HttpGet]
        public IActionResult OperasyonSec()
        {
            // Tüm operasyonları listeler
            var operations = _context.Operations.ToList();
            return View(operations);
        }

        [HttpPost]
        public IActionResult OperasyonSec(int operationId)
        {
            return RedirectToAction("PersonelSec", new { operationId });
        }

        // 2. Adım: Bu operasyonu yapabilen personeli seç
        [HttpGet]
        public IActionResult PersonelSec(int operationId)
        {
            var operation = _context.Operations.Find(operationId);
            if (operation == null) return NotFound();

            // Bu operasyonu yapabilen personelleri bul
            var personeller = _context.Employees
                .Include(e => e.EmployeeOperations)
                .ThenInclude(eo => eo.Operation)
                .Where(e => e.EmployeeOperations.Any(eo => eo.OperationId == operationId))
                .ToList();

            ViewBag.Operation = operation;
            return View(personeller);
        }

        [HttpPost]
        public IActionResult PersonelSec(int operationId, int employeeId)
        {
            return RedirectToAction("TarihSec", new { operationId, employeeId });
        }

        // 3. Adım: Personelin çalışma saatlerine göre uygun zaman seç
        [HttpGet]
        public IActionResult TarihSec(int operationId, int employeeId)
        {
            var operation = _context.Operations.Find(operationId);
            var employee = _context.Employees
                .Include(e => e.WorkingHours)
                .FirstOrDefault(e => e.Id == employeeId);

            if (operation == null || employee == null) return NotFound();

            // Basit bir yaklaşım: Önümüzdeki 7 gün için personelin çalışma saatlerine göre uygun slotları listele.
            // Gerçek bir uygulamada daha sofistike bir takvim yapısı gerekebilir.
            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            // Slot süresi operation.Duration kadar.
            var possibleSlots = new List<DateTime>();

            // Personelin WorkingHours tablo girişlerine göre slot oluşturuyoruz
            // Her WorkingHour kaydı: Day, StartTime, EndTime
            // Örneğin Pazartesi (DayOfWeek.Monday), StartTime=09:00, EndTime=17:00 
            // operation.Duration dakikalık slotlar üretilecek.

            for (DateTime day = today; day <= nextWeek; day = day.AddDays(1))
            {
                var workingHour = employee.WorkingHours.FirstOrDefault(w => w.Day == day.DayOfWeek);
                if (workingHour != null)
                {
                    // Örneğin StartTime=09:00, EndTime=17:00 ve süre=30 dk
                    var start = day.Date + workingHour.StartTime;
                    var end = day.Date + workingHour.EndTime;

                    var duration = TimeSpan.FromMinutes(operation.Duration);

                    for (var slot = start; slot + duration <= end; slot += duration)
                    {
                        // Bu slot müsait mi?
                        bool occupied = _context.Appointments.Any(a => a.EmployeeId == employeeId && a.StartTime == slot && a.IsApproved == true);
                        if (!occupied)
                        {
                            possibleSlots.Add(slot);
                        }
                    }
                }
            }

            ViewBag.Operation = operation;
            ViewBag.Employee = employee;
            return View(possibleSlots);
        }

        [HttpPost]
        public IActionResult TarihSec(int operationId, int employeeId, DateTime startTime)
        {
            // Burada randevuya yönlendirelim
            return RedirectToAction("RandevuOnayla", new { operationId, employeeId, startTime });
        }

        // Son Adım: Randevuyu oluştur
        [HttpGet]
        public IActionResult RandevuOnayla(int operationId, int employeeId, DateTime startTime)
        {
            var operation = _context.Operations.Find(operationId);
            var employee = _context.Employees.Find(employeeId);

            if (operation == null || employee == null) return NotFound();

            // Randevu bilgilerini gösterelim. Kullanıcı onaylarsa POST ile kaydedelim
            ViewBag.Operation = operation;
            ViewBag.Employee = employee;
            ViewBag.StartTime = startTime;

            return View();
        }

        [HttpPost]
        public IActionResult RandevuOnaylaPost(int operationId, int employeeId, DateTime startTime)
        {
            var operation = _context.Operations.Find(operationId);
            var employee = _context.Employees.Find(employeeId);
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            int userId = int.Parse(userIdStr);

            var appointment = new Appointment
            {
                EmployeeId = employeeId,
                OperationId = operationId,
                StartTime = startTime,
                Duration = operation.Duration,
                Price = operation.Price,
                IsApproved = false,
                CustomerId = userId // Burada müşteri bilgisini atıyoruz
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("RandevuAlindi");
        }

        public IActionResult RandevuAlindi()
        {
            return View();
        }

        // Personel için Randevular
        [HttpGet]
        public IActionResult Liste()
        {
            // Giriş yapan personelin Id'sini bul
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            int userId = int.Parse(userIdStr);

            var employee = _context.Employees.FirstOrDefault(e => e.UserId == userId);
            if (employee == null) return Unauthorized();

            // Onaylanmamış veya onaylanmış tüm randevuları listeleyelim
            var appointments = _context.Appointments
                .Include(a => a.Operation)
                .Include(a => a.Employee)
                .ThenInclude(e => e.User)
                .Where(a => a.EmployeeId == employee.Id)
                .OrderBy(a => a.StartTime)
                .ToList();

            return View(appointments);
        }

        [HttpPost]
        public IActionResult RandevuOnay(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            appointment.IsApproved = true;
            _context.SaveChanges();

            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult RandevuIptal(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return NotFound();

            // İptal için dilerseniz Appointment'ı silebilirsiniz
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction("Liste");
        }

        [HttpGet]
        public IActionResult MusteriRandevularim()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized();

            int userId = int.Parse(userIdStr);

            // Müşterinin employee kaydı olmayabilir, ama müşterinin Appointment almak için employee olmaz gerekmez.

            // Eğer Appointment modelinde Randevuyu alan kullanıcı (Customer) bilgisini tutmuyorsak eklememiz gerekir.
            var appointments = _context.Appointments
                .Include(a => a.Employee)
                .ThenInclude(e => e.User)
                .Include(a => a.Operation)
                .Where(a => a.CustomerId == userId)
                .OrderByDescending(a => a.StartTime)
                .ToList();

            return View(appointments);
        }

    }
}
