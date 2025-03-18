using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class CareMoment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Name { get; set; }

        public string? Url { get; set; }

        public byte[]? Picture { get; set; }

        public int TimeDurationInMin { get; set; }

    }
}
