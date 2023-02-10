using Dapper;
using java.lang;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;
using org.omg.IOP;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Encoding = System.Text.Encoding;
using static com.sun.tools.@internal.xjc.reader.xmlschema.bindinfo.BIConversion;

namespace MoonWorkRAPI.Repository
{
    public interface IUserRepository
    {
        public void CreateUser(UserModel user);
        public object Login(UserModel user);
        public object GetUserInfo(UserModel user);
        public Task<IEnumerable<UserModel>> GetUserList();
        public string SelectUserId(string username);
        public string GetId(string name);
        public void GetUserMyInfo(long UserId);
        public void ResetPassword(long UserId);
        public Task UpdateUserInfo(UserModel user);
        public void DeleteUser(long UserId);
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
                "(UserName, Password, Name, Email, Note, IsActive) " +
                "VALUES" +
                "(@UserName, @Password, @Name, @Email, @Note, @IsActive)";

            byte[] array = Encoding.UTF8.GetBytes(user.Password);
            byte[] hashValue;
            string result = string.Empty;

            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashValue = mySHA256.ComputeHash(array);
            }
            for (int i = 0; i < hashValue.Length; i++)
            {
                result += hashValue[i].ToString("x2");
            }


            var param = new DynamicParameters();
            param.Add("UserName", user.UserName);
            param.Add("Password", result);
            param.Add("Name", user.Name);
            param.Add("Email", user.Email);
            param.Add("Note", user.Note);
            param.Add("IsActive", user.IsActive);

            using (var conn = _context.CreateConnection())
            {
                conn.Execute(query, param);
            }
        }

        // 로그인
        public object Login(UserModel user)
        {
            var query = "SELECT COUNT(*) as Result FROM User WHERE UserName = @UserName and Password = @Password";

            byte[] array = Encoding.UTF8.GetBytes(user.Password);
            byte[] hashValue;
            string result = string.Empty;

            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashValue = mySHA256.ComputeHash(array);
            }
            for (int i = 0; i < hashValue.Length; i++)
            {
                result += hashValue[i].ToString("x2");
            }
            Console.WriteLine("result : " + result);

            var name = "SELECT UserName FROM User WHERE UserName = @UserName";
            var pass = "SELECT Password FROM User WHERE UserName = @UserName";

            using (var conn = _context.CreateConnection())
            {
                var username = conn.QuerySingleOrDefault<string>(name, new{ user.UserName });
                var password = conn.QuerySingleOrDefault<string>(pass, new { user.UserName });
                Console.WriteLine("username : " + username);
                Console.WriteLine("password : " + password);
                if (password == result)
                {
                    var param = new DynamicParameters();
                    param.Add("UserName", user.UserName);
                    param.Add("Password", result);

/*                    var all = "SELECT * FROM User WHERE UserName = @UserName AND Password = @Password";
                    Console.WriteLine("all :" + all);
                    
                    var allinfo = conn.QuerySingleOrDefault<UserModel>(all, new {user.UserName, user.Password});
                    Console.WriteLine("allinfo : " + allinfo);
                    
                    var info = conn.QuerySingleOrDefault(all,param);
                    Console.WriteLine("info : " + info);*/

                    var log = conn.QuerySingleOrDefault(query, param);
                    Console.WriteLine("log : " + log);
                    conn.QuerySingleOrDefault(query, param);

                    return log;
                }
                else
                {
                    Console.WriteLine("실패");
                    return "로그인 실패";
                }
            }
        }

        // UserName(ID), Password에 따른 유저 정보
        public object GetUserInfo(UserModel user)
        {
            var query = "SELECT * FROM User WHERE UserName = @UserName AND Password = @Password";

            var name = "SELECT UserName FROM User WHERE UserName = @UserName";
            var pass = "SELECT PassWord FROM User WHERE UserName = @UserName";

            byte[] array = Encoding.UTF8.GetBytes(user.Password);
            byte[] hashValue;
            string result = string.Empty;

            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashValue = mySHA256.ComputeHash(array);
            }
            for (int i = 0; i < hashValue.Length; i++)
            {
                result += hashValue[i].ToString("x2");
            }
            Console.WriteLine("result : " + result);

            using (var conn = _context.CreateConnection())
            {
                /*var use = conn.QuerySingleOrDefaultAsync<UserModel>(query, new{user.UserName, result });*/
                /*Console.WriteLine("user : " + user);*/
                var username = conn.QuerySingleOrDefault<string>(name, new { user.UserName });
                var password = conn.QuerySingleOrDefault<string>(pass, new { user.UserName });


                var param = new DynamicParameters();
                param.Add("UserName", user.UserName);
                param.Add("Password", result);

                var all = "SELECT * FROM User WHERE UserName = @UserName AND Password = @Password";
                Console.WriteLine("all :" + all);

                var allinfo = conn.QuerySingleOrDefault<UserModel>(all, new {user.UserName, user.Password});
                Console.WriteLine("allinfo : " + allinfo);

                var info = conn.QuerySingleOrDefault<UserModel>(all, param);
                Console.WriteLine("info : " + info);

/*                var log = conn.QuerySingleOrDefault(query, result);*/
                return info;
            }
        }

        //전체 유저 리스트
        public async Task<IEnumerable<UserModel>> GetUserList()
        {
            var query = "SELECT * FROM User";

            using (var conn = _context.CreateConnection())
            {
                var list = await conn.QueryAsync<UserModel>(query);
                return list.ToList();
            }
        }


        // 아이디 중복 체크
        public string SelectUserId(string username)
        {
            var query = "SELECT COUNT(*) as Result FROM User WHERE UserName = @UserName";

            using (var conn = _context.CreateConnection())
            {
                var id = conn.QuerySingleOrDefault<string>(query, new { username });

                return id;
            }
        }

        //id 찾기
        public string GetId(string name)
        {
            var query = "SELECT UserName FROM user WHERE Name = @Name";

            using (var conn = _context.CreateConnection())
            {
                var id = conn.QuerySingleOrDefault<string>(query, new { name });
                return id;
            }
        }

        // 내 정보 보기
        public void GetUserMyInfo(long UserId)
        {
            var query = "SELECT u.UserName, u.Name, j.* FROM User u, Job j " +
                "Where u.UserId = @UserId and u.UserId = j.UserId";

            using (var conn = _context.CreateConnection())
            {
                conn.QuerySingleOrDefault<UserModel>(query, new { UserId });
            }
        }

        // 비밀번호 초기화
        public void ResetPassword(long UserId)
        {
            var query = "UPDATE User SET Password = null WHERE UserId = @UserId";

            using(var conn = _context.CreateConnection())
            {
                conn.QuerySingleOrDefault<long>(query, new { UserId });
            }
        }


        // 사용자 정보 변경
        public async Task UpdateUserInfo(UserModel user)
        {
            var query = "UPDATE User SET " +
                "UserName = @UserName, " +
                "Password = @Password, " +
                "Name = @Name, " +
                "Email = @Email, " +
                "Note = @Note, " +
                "IsActive = @IsActive " +
                "Where UserId = @UserId";

            var param = new DynamicParameters();
            param.Add("UserName", user.UserName);
            param.Add("Password", user.Password);
            param.Add("Name", user.Name);
            param.Add("Email", user.Email);
            param.Add("Note", user.Note);
            param.Add("IsActive", user.IsActive);
            param.Add("UserId", user.UserId);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        // 사용자 삭제
        public void DeleteUser(long UserId)
        {
            var query = "DELETE FROM User Where UserId = @userid";

            using (var conn = _context.CreateConnection())
            {
                conn.QuerySingleOrDefault<UserModel>(query, new { UserId });
            }
        }
    }
}
