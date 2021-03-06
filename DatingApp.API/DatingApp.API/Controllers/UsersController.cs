﻿using AutoMapper;
using DatingApp.API.Helpers;
using DatingApp.DomainModels;
using DatingApp.Repository;
using DatingApp.Repository.Helpers;
using DatingApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await repo.GetUserByID(currentUserID);
            userParams.UserID = currentUserID;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "Male" ? "Female" : "Male";
            }
            var users = await repo.GetUsers(userParams);
            var userToReturn = mapper.Map<IEnumerable<UserForDetailViewModel>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(userToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var user = await repo.GetUserByID(id);
            var userToReturn = mapper.Map<UserForDetailViewModel>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateViewModel user)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromrepo = await repo.GetUserByID(id);
            mapper.Map(user, userFromrepo);
            if (await repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/Like/{receptientId}")]
        public async Task<IActionResult> LikeUser(int id, int receptientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var like = await repo.GetLike(id, receptientId);
            if (like != null)
            {
                return BadRequest("You already like this user");
            }
            if (await repo.GetUserByID(receptientId) == null)
            {
                return NotFound();
            }
            like = new Like()
            {
                LikerId = id,
                LikeeId = receptientId
            };
            repo.Add<Like>(like);
            if (await repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to like user");
        }
    }
}