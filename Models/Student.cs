using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Student name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Course is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Course must be between 2 and 50 characters")]
        public string Course { get; set; }



        [Required(ErrorMessage = "Enrollment date is required")]
        public DateTime EnrollmentDate { get; set; }
    }
}


public class StudentDetails
{
    public Student Student { get; set; } = new();
    public IEnumerable<Student> Students { get; set; } = new List<Student>();
}
