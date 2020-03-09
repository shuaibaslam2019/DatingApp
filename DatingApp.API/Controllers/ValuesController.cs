using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    // http://localhost:5000/api/values
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;
        }

        //GET api/values 
        [HttpGet]
        //For handling concurrent request, better to use Async
        public async Task<IActionResult> GetValues()
        {
            var values = await _context.values.ToListAsync();
            return Ok(values);
        }
        [HttpPut("{id}")]
        // Get api/values/5
        public async Task<IActionResult> GetValues(int id)
        {
            // If ID doesn't match it will throw an error without firstOrDefault, so better to use it
            // if use First, it will return the first if it exist otherwise throw an exception

            var values = await _context.values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(values);
        }

    }
}