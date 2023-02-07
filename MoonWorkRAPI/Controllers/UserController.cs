using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;

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

        // 회원가입
        [HttpPost("createuser")]
        public Object CreateUser(UserModel user)
        {
            try
            {
                _userRepo.CreateUser(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 아이디 중복체크
        [HttpGet("idexist")]
        public string SelectUserId(string username)
        {
            var userid = _userRepo.SelectUserId(username);
            return userid;
        }

        // id찾기
        [HttpGet("findid")]
        public string GetId(string name)
        {
            var id = _userRepo.GetId(name);
            return id; ;
        }
    }
}
