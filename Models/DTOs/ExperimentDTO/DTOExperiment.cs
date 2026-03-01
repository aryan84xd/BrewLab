using System.ComponentModel.DataAnnotations;

namespace BrewLab.Models.DTOs.ExperimentDTO
{
    public class DTOExperiment
    {
        public Guid CoffeeId { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public string? BrewMethod { get; set; } = default!;

        [Range(1, 100)]
        public decimal CoffeeWeight { get; set; }

        [Range(1, 2000)]
        public decimal WaterWeight { get; set; }

        [Required]
        public TimeOnly BrewTime { get; set; }

        public string? Remark { get; set; }

        [Range(1, 5)]
        public int Aroma { get; set; }

        [Range(1, 5)]
        public int Acidity { get; set; }

        [Range(1, 5)]
        public int Body { get; set; }

        [Range(1, 10)]
        public int Overall { get; set; }
    }
}
