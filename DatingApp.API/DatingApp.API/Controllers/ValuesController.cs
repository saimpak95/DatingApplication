using System;
using System.Collections.Generic;
using System.Linq;
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
        public  IActionResult Get()
        {
            var values = db.Values.ToList();
            return Ok(values);
        }


        [HttpGet("{id}")]
        public ActionResult<string> GetValue(int id)
        {

            return "Value";
        }
    }
}
