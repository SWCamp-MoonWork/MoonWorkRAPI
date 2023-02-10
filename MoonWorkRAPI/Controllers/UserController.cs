using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using System.Security.Cryptography;

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

        // 로그인
        [HttpPost("login")]
        public object Login(UserModel user)
        {
            try
            {
                var str = _userRepo.Login(user);
                return str;
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        // UserName(ID), Password에 따른 유저 정보
        [HttpPost("getuserinfo")]
        public object GetUserInfo(UserModel user)
        {
            try
            {
                var str = _userRepo.GetUserInfo(user);
                if(str == null)
                {
                    return NotFound();
                }
                return Ok(str);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // 전체 유저 리스트
        [HttpGet("userlist")]
        public async Task<ActionResult<List<UserModel>>> GetUserList()
        {
            try
            {
                var list = await _userRepo.GetUserList();
                return Ok(list);
            }
            catch(Exception ex)
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
            return id;
        }

        // 내 정보 보기
/*        [HttpPost("{UserId}/userinfo")]
        public */

        // 비밀번호 초기화
        [HttpPut("{UserId}/resetpassword")]
        public void ResetPassword(long UserId)
        {
            _userRepo.ResetPassword(UserId);
        }

        // 사용자 정보 변경
        [HttpPut("{UserId}/update")]
        public ActionResult<UserModel> UpdateUserInfo(UserModel user)
        {
            try
            {
                _userRepo.UpdateUserInfo(user);
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 사용자 삭제
        [HttpDelete("{UserId}/delete")]
        public void DeleteUser(long UserId)
        {
            _userRepo.DeleteUser(UserId);
        }
    }
}
