using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Controllers
{
    [ApiController]
    [Route("v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("signin")]
        public Object CreateUser(UserModel user)
        {
            try
            {
                _userRepo.CreateUser();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
