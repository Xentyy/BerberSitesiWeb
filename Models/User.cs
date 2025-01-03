﻿using System.ComponentModel.DataAnnotations;

namespace BerberSite.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Adınızı Giriniz.")]
        [StringLength(50, ErrorMessage = "Adınız en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyadınızı Giriniz.")]
        [StringLength(50, ErrorMessage = "Soyadınız en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Telefon numaranızı giriniz.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numaranız 11 basamaklı olmalıdır.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; } = Role.Customer;

        // Navigation Property
        public virtual Employee? Employee { get; set; }
    }

    public enum Role
    {
        Admin = 1,
        Customer = 2,
        Employee = 3
    }
}
