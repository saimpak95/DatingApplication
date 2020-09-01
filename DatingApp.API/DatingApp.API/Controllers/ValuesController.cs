using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Get()
        {
            var values =await db.Values.ToListAsync();
            return Ok(values);
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var values = await db.Values.FirstOrDefaultAsync(temp=> temp.Id==id);
            return Ok(values);
        }
    }
}
