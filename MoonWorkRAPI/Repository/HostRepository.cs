using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;
using Quartz;

namespace MoonWorkRAPI.Repository
{
    public interface IHostRepository
    {
        public Task<IEnumerable<HostModel>> GetHosts();
        public Task<IEnumerable<HostModel>> GetHost();
        public HostModel GetHostId(long HostId);
        public Task<IEnumerable<HostModel>> GetHost_IsUseTrue();
        /*        public Task<IEnumerable<Host_JobScheduleModel>> GetJob_HostId(long HostId);*/
        /*        public object GetLastRun(long HostId);
                public object GetNextRun(long HostId);*/
        public Task<IEnumerable<Job_HostIdModel>> GetJob_HostId(long HostId);
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
        public async Task<IEnumerable<Job_HostIdModel>> GetJob_HostId(long HostId)
        {
            var getcron = "select distinct s.CronExpression from Host h, Run r, Job j, Schedule s " +
                " Where h.HostId = @HostId and h.HostId = r.HostId and r.JobId = j.JobId and j.JobId = s.JobId";

            var laststartrun = "select r.StartDT From Run r, Host h " +
                " Where h.HostId = @HostId and h.HostId = r.HostId order by RunId desc limit 0,1 ";

            using (var conn = _context.CreateConnection())
            {
                var cron = conn.QuerySingleOrDefault<string>(getcron, new { HostId });
                var start = conn.QuerySingleOrDefault<DateTime>(laststartrun, new { HostId });
                var expression = new CronExpression(cron);
                DateTimeOffset? time = expression.GetTimeAfter(start);

                Console.WriteLine("time : " + time);

                string sub = time.ToString();
                Console.WriteLine("sub : " + sub);

                string ss = sub.Substring(0, 10);
                Console.WriteLine("ss : " + ss);

                string sd = sub.Substring(14,8);
                Console.WriteLine("sd : " + sd);

                string total = ss + " " + sd;
                Console.WriteLine("total : " + total);

                var query = "SELECT distinct j.JobId, j.JobName, " +
                    "(SELECT convert(r.EndDT, datetime(3)) " +
                    "From Run r, Host h " +
                    "where EndDT is not null and h.HostId = @HostId and h.HostId = r.HostId " +
                    "order by RunId desc limit 1) as LastRun, '" +
                    total + "' as NextRun " +
                    "FROM Job j, Run r, Host h " +
                    "WHERE j.JobId = r.JobId and r.HostId = h.HostId and h.HostId = @HostId";

                var str = await conn.QueryAsync<Job_HostIdModel>(query, new { HostId });

                return str.ToList();
            }
        }

/*        // HostId 에 따른 Job의 Last Run Select
        public object GetLastRun(long HostId)
        {
            var last = "SELECT convert(r.EndDT, datetime(3)) " +
                " From Run r, Host h " +
                " where EndDT is not null and h.HostId = @HostId and h.HostId = r.HostId " +
                " order by RunId desc limit 1";

            using (var conn = _context.CreateConnection())
            {
                var str = conn.QuerySingleOrDefault<object>(last, new { HostId });
                return str;
            }
        }*/
/*
        //HostId에 따른 Job의 Next Run Select
        public object GetNextRun(long HostId)
        {
            var getcron = "select distinct s.CronExpression from Host h, Run r, Job j, Schedule s " +
                " Where h.HostId = @HostId and h.HostId = r.HostId and r.JobId = j.JobId and j.JobId = s.JobId";

            var laststartrun = "select r.StartDT From Run r, Host h " +
                " Where h.HostId = @HostId and h.HostId = r.HostId order by RunId desc limit 0,1 ";

            using (var conn = _context.CreateConnection())
            {
                var str = conn.QuerySingleOrDefault<string>(getcron, new { HostId });
                var start = conn.QuerySingleOrDefault<DateTime>(laststartrun, new { HostId });
                var expression = new CronExpression(str);
                DateTimeOffset? time = expression.GetTimeAfter(start);
                return time;
            }
        }*/

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