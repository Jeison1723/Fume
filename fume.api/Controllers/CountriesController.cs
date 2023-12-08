﻿using fume.api.Data;
using fume.shared.Enttities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fume.api.Controllers
{
    [ApiController]
    [Route("/api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _Context;
        public CountriesController(DataContext context)
        {
            _Context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _Context.Countries
                .Include(x => x.States)
                .ToListAsync());
        }
        [HttpGet("Full")]
        public async Task<ActionResult> GetFull()
        {
            return Ok(await _Context.Countries
                .Include(x => x.States!)
                .ThenInclude(x => x.Cities)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Post(Country country)
        {
            try
            {
                _Context.Add(country);
                await _Context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbupdateexception)
            {
                if(dbupdateexception.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un pais con el mismo nombre.");
                }

                return BadRequest(dbupdateexception.Message);
             
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var country = await _Context.Countries
                .Include(x => x.States)
                .ThenInclude (x => x.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (country is null)
            {
                return NotFound();
            }

            return Ok(country);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Country country)
        {
            try
            {
                _Context.Update(country);
                await _Context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbupdateexception)
            {
                if (dbupdateexception.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un pais con el mismo nombre.");
                }

                return BadRequest(dbupdateexception.Message);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var afectedRows = await _Context.Countries
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            if (afectedRows == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
