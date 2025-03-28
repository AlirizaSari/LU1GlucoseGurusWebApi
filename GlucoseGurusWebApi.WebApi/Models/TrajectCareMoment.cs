﻿using System.ComponentModel.DataAnnotations;

namespace GlucoseGurusWebApi.WebApi.Models
{
    public class TrajectCareMoment
    {
        [Required]
        public Guid TrajectId { get; set; }

        [Required]
        public Guid CareMomentId { get; set; }

        public string? Name { get; set; }

        [Required]
        public int Step { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
