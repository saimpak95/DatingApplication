using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DatingApp.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext db;

        public ValuesController(DataContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await db.Values.ToListAsync();
            return Ok(values);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var valuebyID = await db.Values.FirstOrDefaultAsync(temp => temp.Id == id);
            return Ok(valuebyID);
        }
    }
}
