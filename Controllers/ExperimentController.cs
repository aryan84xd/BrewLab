using BrewLab.Models.DTOs.CoffeeDTO;
using BrewLab.Models.DTOs.ExperimentDTO;
using BrewLab.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrewLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperimentController : BaseApiController
    {
        public ExperimentController(AppDbContext db) : base(db) { }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<DTOExperiment>>> GetCoffeeExperiments(Guid coffeeId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();
            var coffeeExists = await _db.Coffees
            .AnyAsync(c => c.Id == coffeeId && c.UserId == user.Id);
            if (!coffeeExists)
                return NotFound("Coffee not found for this user.");

            var experiments = await _db.Experiments
        .Where(e => e.CoffeeId == coffeeId && e.UserId == user.Id)
        .Select(e => new DTOExperiment
        {
            Date = e.Date,
            Acidity = e.Acidity,
            Aroma = e.Aroma,
            Body = e.Body,
            BrewMethod = e.BrewMethod,
            BrewTime = e.BrewTime,
            CoffeeWeight = e.CoffeeWeight,
            WaterWeight = e.WaterWeight,
            Overall = e.Overall,
            Remark = e.Remark
        })
        .ToListAsync();

            return Ok(experiments);


        }

        [HttpPost]
        public async Task<ActionResult<DTOExperiment>> PostExperiment(DTOExperiment experimentDto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();
            var coffee = await _db.Coffees.FirstOrDefaultAsync(c => c.Id == experimentDto.CoffeeId && c.UserId == user.Id);
            if (coffee == null)
                return NotFound("Coffee not found for the user.");
            var experiment = new Experiment {
                Coffee = coffee,
                CoffeeId = experimentDto.CoffeeId,
                BrewMethod = experimentDto.BrewMethod,
                CoffeeWeight = experimentDto.CoffeeWeight,
                WaterWeight = experimentDto.WaterWeight,
                BrewTime = experimentDto.BrewTime,
                Remark = experimentDto.Remark,
                Aroma = experimentDto.Aroma,
                Acidity = experimentDto.Acidity,
                Body = experimentDto.Body,
                Overall = experimentDto.Overall
            };


            _db.Experiments.Add(experiment);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetCoffee", new { id = coffee.Id }, experimentDto.Date);
        }

    }


}
