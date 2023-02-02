using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IUserRepository
    {
        
    }
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        // 회원가입
        public void CreateUser(UserModel user)
        {
            var query = "INSERT INTO User" +
                "(UserName, Password, Name) " +
                "VALUES" +
                "(@UserName, @Password, @Name)";

            var param = new DynamicParameters();
            param.Add("UserName", user.UserName);
            param.Add("Password", user.Password);
            param.Add("Name", user.Name);

            using (var conn = _context.CreateConnection())
            {
                conn.Execute(query, param);
            }
        }

        // 아이디 중복 체크
        
    }
}
