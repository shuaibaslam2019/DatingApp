using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Repositories;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("username already exists");
            }
            var create_user = _mapper.Map<User>(userForRegisterDto);
            // var create_user = new User
            // {
            //     Username = userForRegisterDto.Username
            // };

            var created_user = await _repo.Register(create_user, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(created_user); // to avoid sending password information

            // return StatusCode(201);
            return CreatedAtRoute("GetUser", new { Controller = "Users", id = created_user.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user_login = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if (user_login == null)
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user_login.Id.ToString()),
                new Claim(ClaimTypes.Name, user_login.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.
                                    GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token_descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var token_handler = new JwtSecurityTokenHandler();

            var token = token_handler.CreateToken(token_descriptor);

            var user = _mapper.Map<UserForListDto>(user_login);

            return Ok(new
            {
                token = token_handler.WriteToken(token),
                user

            });
        }
    }
}