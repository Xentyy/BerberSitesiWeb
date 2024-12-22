using BerberSite.Models;
using System.Collections.Generic;

namespace BerberSite.ViewModels
{
    public class PersonelDuzenleViewModel
    {
        public Employee Employee { get; set; }
        public List<Operation> AllOperations { get; set; }
        public WorkingHour NewWorkingHour { get; set; }
    }
}
