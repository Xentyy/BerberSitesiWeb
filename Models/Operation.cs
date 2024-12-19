using System.ComponentModel.DataAnnotations;

namespace BerberSite.Models
{
    public class Operation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "İşlemin adını giriniz.")]
        public string OperationName { get; set; }

        [Required(ErrorMessage = "Fiyatı giriniz.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat sıfır veya daha büyük olmalıdır.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Süreyi giriniz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Süre en az 1 dakika olmalıdır.")]
        public int Duration { get; set; }

        // Koleksiyonu başlatıyoruz
        public ICollection<EmployeeOperation> EmployeeOperations { get; set; } = new List<EmployeeOperation>();
    }
}
