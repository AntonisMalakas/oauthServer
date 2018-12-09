using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication;
using Authentication.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationTokenTest.Controllers
{
    [Route("api/[controller]")]
    public class TokenAuthController : Controller
    {
        private IAuthenticationProcess _authenticationProcess;
        public TokenAuthController(IAuthenticationProcess authenticationProcess)
        {
            this._authenticationProcess = authenticationProcess;
        }
        // GET: api/<controller>
        [HttpPut("Login")]
        public Object Login([FromBody]User user)
        {
            var response = this._authenticationProcess.LoginProcess(user);
            return response;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody]User user)
        {
            try
            {
                // save 
                var response = this._authenticationProcess.RegisterProcess(user);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }
    }
}
