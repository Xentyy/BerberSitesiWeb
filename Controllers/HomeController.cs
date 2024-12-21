using BerberSite.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BerberSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Hizmetler()
        {
            var operations = _context.Operations.ToList();
            return View(operations);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
