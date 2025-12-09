using System.ComponentModel.DataAnnotations;

namespace TimeTrackingApp.Models.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Position { get; set; }

        public string Department { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must have at least 6 characters.")]
        public string Password { get; set; }
    }
}

