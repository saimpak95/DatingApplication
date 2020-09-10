using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Repository;
using DatingApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repo.GetUsers();
            var userToReturn = mapper.Map<IEnumerable<UserForDetailViewModel>>(users);
            return Ok(userToReturn);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var user = await repo.GetUserByID(id);
            var userToReturn = mapper.Map<UserForDetailViewModel>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UserForUpdateViewModel user)
        {
           if (id!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromrepo = await repo.GetUserByID(id);
            mapper.Map(user, userFromrepo);
            if(await repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}
