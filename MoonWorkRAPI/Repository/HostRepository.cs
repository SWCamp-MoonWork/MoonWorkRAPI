using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IHostRepository
    {
        public Task<IEnumerable<HostModel>> GetHosts();
        public Task<IEnumerable<HostModel>> GetHost();
        public HostModel GetHostId(long HostId);
        public Task<IEnumerable<HostModel>> GetHost_IsUseTrue();
        public Task<IEnumerable<Host_JobScheduleModel>> GetJob_HostId(long HostId);
        public Task UpdateHost(HostModel host);
    }
    public class HostRepository : IHostRepository
    {
        private readonly DapperContext _context;

        public HostRepository(DapperContext context)
        {
            _context = context;
        }

        //host 전부 다 가져오기
        public async Task<IEnumerable<HostModel>> GetHosts()
        {
            var query = "SELECT * FROM Host";

            using (var connection = _context.CreateConnection())
            {
                var host = await connection.QueryAsync<HostModel>(query);
                return host.ToList();
            }
        }

        //run 시킬 host 찾기
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

        // hostId 찾기
        public HostModel GetHostId(long hostid)
        {
            var query = "SELECT HostId FROM Host WHERE HostId = @HostId";

            using (var connection = _context.CreateConnection())
            {
                var host = connection.QuerySingleOrDefault<HostModel>(query, new { hostid });
                return host;
            }
        }


        // IsUse가 true인 Host 추출
        public async Task<IEnumerable<HostModel>> GetHost_IsUseTrue()
        {
            var query = "SELECT * FROM Host WHERE IsUse = true";
;
            using (var connection = _context.CreateConnection())
            {
                var host = await connection.QueryAsync<HostModel>(query);
                return host.ToList();
            }
        }

        // HostId에 따른 Job의 정보 추출
        public async Task<IEnumerable<Host_JobScheduleModel>> GetJob_HostId(long HostId)
        {
            var query = "SELECT DISTINCT j.JobId, j.JobName, s.ScheduleStartDT, s.ScheduleEndDT " +
                "FROM Job j, Schedule s, Run r, Host h " +
                "Where h.HostId = @HostId and h.HostId = r.HostId and r.JobId = j.JobId and j.JobId = s.JobId";

            using (var conn = _context.CreateConnection())
            {
                var str = await conn.QueryAsync<Host_JobScheduleModel>(query, new { HostId });
                return str.ToList();
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

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
    }
}