using BrewLab.Models.Entities;
using BrewLab.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BrewLab.Models.DTOs.CoffeeDTO;

namespace BrewLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeesController : BaseApiController
    {
       


        public CoffeesController(AppDbContext db) : base(db) { }

       

        // GET: api/Coffees
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coffee>>> GetCoffee()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var coffees = await _db.Coffees
        .Where(c => c.UserId == user.Id)
        .Select(c => new DTOCoffee
        {
            Id = c.Id,
            Name = c.Name,
            Brand = c.Brand,
            Roast = c.Roast,
            Origin = c.Origin,
            TastingNotes = c.TastingNotes
        })
        .ToListAsync();

            return Ok(coffees);
        }

        // GET: api/Coffees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTOCoffee>> GetCoffee(Guid id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var coffee = await _db.Coffees
        .Where(c => c.Id == id && c.UserId == user.Id)
        .Select(c => new DTOCoffee
        {
            Id = c.Id,
            Name = c.Name,
            Brand = c.Brand,
            Roast = c.Roast,
            Origin = c.Origin,
            TastingNotes = c.TastingNotes
        })
        .FirstOrDefaultAsync();

            if (coffee == null)
                return NotFound();

            return Ok(coffee);


        }

        // PUT: api/Coffees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCoffee(Guid id, Coffee coffee)
        //{
        //    if (id != coffee.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _db.Entry(coffee).State = EntityState.Modified;

        //    try
        //    {
        //        await _db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CoffeeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Coffees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DTOCoffee>> PostCoffee(DTOCoffee coffeeDTO)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var coffee = new Coffee
            {
                
                Name = coffeeDTO.Name,
                Brand = coffeeDTO.Brand,
                Roast = coffeeDTO.Roast,
                Origin = coffeeDTO.Origin,
                TastingNotes = coffeeDTO.TastingNotes,
                UserId = user.Id,
               
            };


            _db.Coffees.Add(coffee);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetCoffee", new { id = coffee.Id },coffeeDTO.Name);
        }



        //// DELETE: api/Coffees/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCoffee(Guid id)
        //{
        //    var coffee = await _db.Coffee.FindAsync(id);
        //    if (coffee == null)
        //    {
        //        return NotFound();
        //    }

        //    _db.Coffee.Remove(coffee);
        //    await _db.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CoffeeExists(Guid id)
        //{
        //    return _db.Coffee.Any(e => e.Id == id);
        //}
    }
}
