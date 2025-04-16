using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Models.Entities
{
    public class CompanyEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        [StringLength(150, ErrorMessage = "Street address cannot be longer than 150 characters.")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(20, ErrorMessage = "State cannot be longer than 20 characters.")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Postal code must be a valid 5-digit or 9-digit ZIP code (e.g., 12345 or 12345-6789).")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }

    }
}
