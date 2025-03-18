﻿using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string? Text { get; set; }

        public Guid ParentGuardianId { get; set; }

        public Guid PatientId { get; set; }

    }
}
