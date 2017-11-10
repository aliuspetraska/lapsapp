using System.ComponentModel.DataAnnotations;

namespace LapsWebApi.Models
{
    public class Track
    {
        [Key]
        [Required]
        [MaxLength]
        public string Id { get; set; }

        [Required]
        [MaxLength]
        public string Coordinates { get; set; }

        [Required]
        [MaxLength]
        public double Distance { get; set; }

        [Required]
        [MaxLength]
        public double Thumbnail { get; set; }
    }
}
