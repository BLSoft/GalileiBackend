using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Owin_Auth.Id;
using Owin_Auth.Utils;

namespace Galilei.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DataContext _context;
        private IUserManager _userManager;
        private IValidationManager _validationManager;

        public UserController(DataContext context, IUserManager userManager, IValidationManager val)
        {
            _context = context;
            _userManager = userManager;
            _validationManager = val;
        }

        [HttpGet]
        [Route("Validate/{longId}")]
        public async Task<IActionResult> ValidateUser(string longId)
        {
            if (await _validationManager.IsLiveValidation(_context,longId))
            {
                var vs = await _validationManager.GetValidationForId(_context,longId);
                var username = vs.Username;
                await _userManager.ValidateUser(_context,username);
                return Ok();
            }
            else
            {
                return BadRequest(new {Message = "user.val.bad"});
            }

            
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserData")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
               
                var username = User.FindFirst(ClaimTypes.Name).Value;
                var user = await _userManager.GetUserByUsername(_context,username);
                if (user != null)
                {
                    user.PasswordHash = "";
                    return Ok(user);
                }
                else
                {
                    return BadRequest(new {message = "usr.getdata.notfound"});
                }

            }
            catch (Exception e)
            {
                return this.ReportError(e);
            }
        }

        public class GetUserDataRequest
        {
            public string Username { get; set; }
        }
    }
}