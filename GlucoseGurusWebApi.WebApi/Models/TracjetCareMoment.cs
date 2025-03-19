using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class TracjetCareMoment
    {
        [Required]
        public Guid TrajectId { get; set; }

        [Required]
        public Guid CareMomentId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int Step { get; set; }
    }
}
