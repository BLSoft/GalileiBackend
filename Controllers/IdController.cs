using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Galilei.Server.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Owin_Auth.Id;
using Owin_Auth.Utils;

namespace Owin_Auth.Controllers
{
    [Route("id")]
    [ApiController]
    public class IdController : ControllerBase
    {
        private IUserManager _userManager;
        private DataContext _context;
        private IValidationManager _validManager;
        private IConfiguration _config;
        private ILogger<IdController> _logger;

        public IdController(IUserManager userManager, IValidationManager manager, DataContext ctx, IConfiguration config, ILogger<IdController> logger)
        {
            _userManager = userManager;
            _validManager = manager;
            _context = ctx;
            _config = config;
            _logger = logger;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (request.Username.CheckNull() || request.Password.CheckNull() || request.Email.CheckNull())
                {
                    return BadRequest(new {message = "id.reg.blankfields"});
                }

                var resp = await _userManager.RegisterNewUser(_context,request.Username, request.Password, request.Email);
                if (resp == UserRegistrationResult.SUCCESS)
                {
                    var vid = await _validManager.RegisterUserForValidation(_context, request.Username);
                    SendValidationEmail(vid,request.Username,request.Email);
                    return Ok();
                }else if (resp == UserRegistrationResult.EXISTS)
                {
                    return BadRequest(new {message = "id.reg.userexists"});
                }
                else
                {
                    return BadRequest(new {message = "id.reg.err"});
                }
            }
            catch (Exception e)
            {
                return this.ReportError(e);
            }
        }

        private void SendValidationEmail(string vid, string username,string email)
        {
            string url = "https://" + Request.Host.Host + ":" + (Request.Host.Port ?? 80) + "/api/user/validate/" + vid.ToLower();
            string emailContent = $@"Hello {username},

Thanks for registering to the Galilei IoT Framework.
Please activate your account by clicking to the following link. This is required because we have to check if the provided email address is correct.
Thanks for the cooperation.
{url}

The Galilei Team";

            EmailHelper.SendEmail("Galilei Mailer Daemon",email,"Please validate your email address",emailContent);
            _logger.LogInformation("Email sent");
        }


        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                if (request.Username.CheckNull() || request.Password.CheckNull())
                {
                    return BadRequest(new {message = "id.auth.blankfields"});
                }

               (LoginResult status,User user) resp = await _userManager.LoginUser(_context,request.Username, request.Password);

                if (resp.status == LoginResult.SUCCESS)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, request.Username),
                        new Claim(JwtRegisteredClaimNames.Jti,new Guid().ToString()), 
                        new Claim("Role",resp.user.Role.ToString()) 
                    };

                    JwtSecurityToken token = new JwtSecurityToken(_config["Tokens:Issuer"],_config["Tokens:Audience"],claims,DateTime.Now,DateTime.Now.AddMinutes(15),new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(_config["Tokens:SecretKey"])),SecurityAlgorithms.HmacSha256));

                    var result = new { Token = new JwtSecurityTokenHandler().WriteToken(token), Expires = 900};
                    return Ok(result);
                }else if (resp.status == LoginResult.BAD_PASSWORD)
                {
                    return BadRequest(new {Message = "id.auth.badpw"});
                }
                else if (resp.status == LoginResult.BAD_USER)
                {
                    return BadRequest(new {Message = "id.auth.baduser"});
                }
                else if (resp.status == LoginResult.LOCKED)
                {
                    return BadRequest(new {Message = "id.auth.locked"});
                }else if (resp.status == LoginResult.NOT_ACTIVATED)
                {
                    return BadRequest(new {Message = "id.auth.inactive"});
                }else if (resp.status == LoginResult.ERROR)
                {
                    return BadRequest(new {Message = "id.auth.err"});
                }

                return BadRequest(new {Message = "Unknown error"});

            }
            catch (Exception e)
            {
                return this.ReportError(e);
            }

        }

        
        

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            
        }

        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
    }
}