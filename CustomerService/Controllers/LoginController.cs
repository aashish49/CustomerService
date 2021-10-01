using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using CustomerService.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route ("[Controller]")]
    public class LoginController
        : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IList<Credential> _appUsers
            = new List<Credential>
            {
                new Credential { FullName = "Admin User" , UserName = " admin", Password = "1234", UserRole ="Admin"},
                new Credential { FullName = "Test User" , UserName = "User" , Password = "1234" , UserRole = "User" }
            };

        public LoginController (IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Credential credential)
        {
            // Default response.
            IActionResult response = Unauthorized();

            // Authenticate the user with the supplied ctredentials.
            var user = AuthenticateUser(credential);

            // Was authentication successfull?
            if (user != null)
            {
                // Yes. Generate the JWT.
                var tokenString = GenerateJWTToken (user);

                // Prepare a 200 - OK response.
                response = Ok(
                    new
                    {
                        token = tokenString,
                        userDetails = user,
                    });
            }

            return response;
        }

        Credential AuthenticateUser (Credential loginCredentials)
        {
            Credential user
                = _appUsers.SingleOrDefault(x =>
                x.UserName == loginCredentials.UserName
                && x.Password == loginCredentials.Password);

            return user;
        }

        private string GenerateJWTToken ( Credential credential)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Set the claims which will also include "roles".
            var claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Sub, credential.UserName),
                new Claim ("fullName", credential.FullName),
                new Claim ("role", credential.UserRole),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken (
                issuer: _config["Jwt:Issuer"],
                audience: _config ["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes (30),
                signingCredentials : signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}