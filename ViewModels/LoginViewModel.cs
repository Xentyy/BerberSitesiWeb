using System.ComponentModel.DataAnnotations;

namespace BerberSite.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-Posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
