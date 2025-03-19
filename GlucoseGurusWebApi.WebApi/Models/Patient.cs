using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public int Avatar { get; set; }

        [Required]
        public Guid ParentGuardianId { get; set; }

        [Required]
        public Guid TrajectId { get; set; }

        public Guid DoctorId { get; set; }
    }
}
