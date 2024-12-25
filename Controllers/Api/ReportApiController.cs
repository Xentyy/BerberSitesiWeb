using BerberSite.Data;
using BerberSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BerberSite.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reportapi/haftaninpersoneli
        [HttpGet("haftaninpersoneli")]
        public IActionResult GetHaftaninPersoneli()
        {
            DateTime startDate = DateTime.Today.AddDays(-7);

            // Son 7 gün içerisinde onaylanmış randevular
            var lastWeekAppointments = _context.Appointments
                .Where(a => a.IsApproved == true && a.StartTime >= startDate)
                .ToList();

            if (!lastWeekAppointments.Any())
            {
                return Ok(new
                {
                    Message = "Son 7 günde onaylanmış randevu bulunamadı. Haftanın elemanı yok."
                });
            }

            // Personel bazında grupla ve toplam kazancı hesapla
            var employeeEarnings = lastWeekAppointments
                .GroupBy(a => a.EmployeeId)
                .Select(g => new {
                    EmployeeId = g.Key,
                    TotalEarnings = g.Sum(x => x.Price)
                })
                .OrderByDescending(e => e.TotalEarnings)
                .FirstOrDefault(); // En çok kazandıranı al

            if (employeeEarnings == null)
            {
                return Ok(new
                {
                    Message = "Haftanın elemanı bulunamadı."
                });
            }

            // İlgili personeli bul
            var employee = _context.Employees
                .Include(e => e.User)
                .FirstOrDefault(e => e.Id == employeeEarnings.EmployeeId);

            if (employee == null)
            {
                return Ok(new
                {
                    Message = "Personel bulunamadı."
                });
            }

            // JSON olarak geri döndür
            return Ok(new
            {
                EmployeeName = employee.Name + " " + employee.Surname,
                TotalEarnings = employeeEarnings.TotalEarnings,
                Message = "Haftanın elemanı bulundu."
            });
        }
    }
}
