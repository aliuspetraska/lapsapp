using System;
using System.ComponentModel.DataAnnotations;

namespace LapsWebApi.Models
{
    public class Track
    {
        [Key]
        [Required]
        [StringLength(255)]
        public string Id { get; set; }

        [Required]
        [MaxLength]
        public string Coordinates { get; set; }

        [Required]
        [MaxLength]
        public double Distance { get; set; }

        [Required]
        [StringLength(255)]
        public string Thumbnail { get; set; }

        [Required]
        [MaxLength]
        public DateTime Timestamp { get; set; }
    }
}
