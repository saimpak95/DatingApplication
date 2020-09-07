using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.DomainModels;
using DatingApp.Repository;
using DatingApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            this.repo = repo;
            this.config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterViewModel user)
        {
            
            var username = user.UserName.ToLower();
            
            if(await repo.UserExists(username))
            {
                return BadRequest("User already exist!");
            }
            var userToCreate = new User
            {
                UserName = username,

            };
            var createdUser = await repo.Register(userToCreate, user.Password);
            return StatusCode(201); // we will fix it later
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginViewModel user)
        {
            
                var userFromRepo = await repo.Login(user.UserName, user.Password);
                if (userFromRepo == null)
                    return Unauthorized();

                // Claims
                var claims = new[]
                 {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };

                // Key
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

                // Credentials
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                /*Security Token descriptor
                 Which is going to contain our claims, our expiry date for our Tokens and the Signing credentials */

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(2),
                    SigningCredentials = creds
                };

                /* And as well as Token Descriptor we need a token handler */
                var tokenHandler = new JwtSecurityTokenHandler();

                // Token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });

           
        }
    }
}
