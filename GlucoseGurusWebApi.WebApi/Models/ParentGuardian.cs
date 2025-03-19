using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class ParentGuardian
    {
        public const int MaxNumberOfParentGuardians = 1;

        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(450)]
        public string? UserId { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }
    }
}
