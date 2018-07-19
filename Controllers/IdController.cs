using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public IdController(IUserManager userManager, IValidationManager manager, DataContext ctx, IConfiguration config)
        {
            _userManager = userManager;
            _validManager = manager;
            _context = ctx;
            _config = config;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (CheckNull(request.Username) || CheckNull(request.Password) || CheckNull(request.Email))
                {
                    return BadRequest(new {message = "id.reg.blankfields"});
                }

                var resp = await _userManager.RegisterNewUser(_context,request.Username, request.Password, request.Email);
                if (resp == UserRegistrationResult.SUCCESS)
                {
                    var vid = await _validManager.RegisterUserForValidation(_context, request.Username);
                    return Ok(new {validationId = vid});
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
                return ReportError(e);
            }
        }

        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                if (CheckNull(request.Username) || CheckNull(request.Password))
                {
                    return BadRequest(new {message = "id.auth.blankfields"});
                }

               (LoginResult status,User user) resp = await _userManager.LoginUser(_context,request.Username, request.Password);

                if (resp.status == LoginResult.SUCCESS)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, request.Username),
                        new Claim("Role",resp.user.Role.ToString()) 
                    };

                    JwtSecurityToken token = new JwtSecurityToken(_config["Tokens:Issuer"],_config["Tokens:Audience"],claims,DateTime.Now,DateTime.Now.AddMinutes(15));

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
                return ReportError(e);
            }

        }

        //Common error reporter
        IActionResult ReportError(Exception e)
        {
            return BadRequest(new {message = $"err.custom {e.GetType().ToString()}:{e.Message}"});
        }
        //NullCheck
        public bool CheckNull(string o)
        {
            return o == null || o.Trim().Length == 0;
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