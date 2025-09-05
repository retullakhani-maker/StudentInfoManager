using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Students
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }

        [Required, Phone, StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; }

        // 🔹 Foreign Key for City
        [Required]
        [Display(Name = "City")]
        public int CityId { get; set; }
        public Cities City { get; set; }

        // 🔹 Foreign Key for State
        [Required]
        [Display(Name = "State")]
        public int StateId { get; set; }
        public States State { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Birth Place")]
        public string Birthplace { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(200)]
        [Display(Name = "Profile Photo")]
        public string? PhotoPath { get; set; }

        public string? ApplicationUserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
