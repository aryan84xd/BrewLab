using System.ComponentModel.DataAnnotations;

namespace BrewLab.Models.Entities
{
    public class Experiment
    {
        public Guid Id { get; set; } =  Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.UtcNow;
    
        public string? BrewMethod { get; set; }

        public decimal CoffeeWeight { get; set; }
        public decimal WaterWeight { get; set; }
        public TimeOnly BrewTime { get; set; }

        public string? Remark { get; set; }

  
        public int Aroma { get; set; }
        
        public int Acidity { get; set; }

        public int Body { get; set; }
        public int Overall { get; set; }



        // Foreign key to Coffee
        public Guid CoffeeId { get; set; }
        public Coffee? Coffee { get; set; }

        public Guid UserId { get; set; }
         public User? User { get; set; }
    }
}
