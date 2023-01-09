using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IHostRepository
    {
        public Task<IEnumerable<HostModel>> GetHost();
        public Task UpdateHost(HostModel host);
    }
    public class HostRepository : IHostRepository
    {
        private readonly DapperContext _context;

        public HostRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HostModel>> GetHost()
        {
            var query = "SELECT HostId, HostName, IsUse, Role, EndPoint, Note, SaveDate, UserId "
                + " FROM Host WHERE IsUse = false";

            using (var connection = _context.CreateConnection())
            {
                var host = await connection.QueryAsync<HostModel>(query);
                    return host.ToList();
            }
        }

        public async Task UpdateHost(HostModel host)
        {
            var query = "UPDATE Host SET "
                + " HostName = @HostName, "
                + " IsUse = @IsUse, "
                + " Role = @Role, "
                + " EndPOint = @EndPoint, "
                + " Note = @Note, "
                + " SaveDate = @SaveDate, "
                + " UserId = @UserId "
                + " WHERE HostId = @HostId";

            var param = new DynamicParameters();
            param.Add("HostId", host.HostId);
            param.Add("HostName", host.HostName);
            param.Add("IsUse", host.IsUse);
            param.Add("Role", host.Role);
            param.Add("EndPoint", host.EndPoint);
            param.Add("Note", host.Note);
            param.Add("SaveDate", host.SaveDate);
            param.Add("UserId", host.UserId);

            using(var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
    }
}
