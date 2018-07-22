using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public UserController(DataContext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Validate/{longId}")]
        public async Task<IActionResult> ValidateUser(string longId)
        {
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserData/{userid}")]
        public async Task<IActionResult> GetUserData(int userid)
        {
            try
            {

                var user = await _userManager.GetUserById(_context,userid);
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