using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BerberSite.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to User
        [Required]
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required(ErrorMessage = "Adınızı giriniz.")]
        [StringLength(50, ErrorMessage = "Adınız en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyadınızı giriniz.")]
        [StringLength(50, ErrorMessage = "Soyadınız en fazla 50 karakter olabilir.")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }

        // Koleksiyonları başlatıyoruz
        public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public ICollection<EmployeeOperation> EmployeeOperations { get; set; } = new List<EmployeeOperation>();
    }
}
