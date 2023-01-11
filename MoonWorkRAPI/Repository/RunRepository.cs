using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IRunRepository
    {
        public Task CreateRun(RunModel run);
        public Task<RunModel> GetRunInfo(int RunId);
        public Task<RunModel> GetRun();
        public Task<IEnumerable<RunModel>> GetRunbyJobId(int JobId);
        public Task<IEnumerable<RunModel>> GetRunbyDate(DateTime FromDT, DateTime ToDT);
        public Task UpdateRun(RunModel run);
    }

    public class RunRepository : IRunRepository
    {
        private readonly DapperContext _context;

        public RunRepository(DapperContext context)
        {
            _context = context;
        }

        //run 시킨 후 run에 대한 정보 기록
        public async Task CreateRun(RunModel run)
        {
            var query = "INSERT INTO Run "
                + " (RunId, StartDT, EndDT, State, HostId, SaveDate) "
                + " VALUES "
                + " (@RunId, @StartDT, @EndDT, @State, @HostId, @SaveDate)";

            var param = new DynamicParameters();
            param.Add("RunId", run.RunId);
            param.Add("StartDT", run.StartDT);
            param.Add("EndDT", run.EndDT);
            param.Add("State", run.State);
            param.Add("HostId", run.HostId);
            param.Add("SaveDate", run.SaveDate);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }


        // 특정 runid에 대한 정보 가져오기
        public async Task<RunModel> GetRunInfo(int RunId)
        {
            var query = "SELECT RunId, StartDT, EndDT, State, JobId, HostId, SaveDate "
                + " FROM Run WHERE RunId = @RunId";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QuerySingleOrDefaultAsync<RunModel>(query, new { RunId });
                return run;
            }
        }

        // run select 업데이트에 필요한거
        public async Task<RunModel> GetRun()
        {
            var query = "SELECT RunId, StartDT, EndDT, State, JobId, HostId, SaveDate "
                + " FROM Run ";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryFirstOrDefaultAsync<RunModel>(query);
                return run;
            }
        }

        //특정 JobId로 인해 발생한 run 조회
        public async Task<IEnumerable<RunModel>> GetRunbyJobId(int JobId)
        {
            var query = "SELECT RunId, StartDT, EndDT, State, JobId, HostId, SaveDate "
                + " FROM Run WHERE JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new { JobId });
                return run.ToList();
            }
        }

        //일정 기간동안의 모든 run 조회
        public async Task<IEnumerable<RunModel>> GetRunbyDate(DateTime FromDT, DateTime ToDT)
        {

            var query = "SELECT RunId, StartDT, EndDT, State, JobId, HostId, SaveDate "
                + " FROM Run WHERE StartDT >= @FromDT AND EndDT <= @ToDT";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new { FromDT, ToDT });
                return run.ToList();
            }
        }

        // 특정 RunId에 대한 정보 업데이트
        public async Task UpdateRun(RunModel run)
        {

            var query = "UPDATE Run SET " +
                " StartDT = @StartDT, " +
                " EndDT = @EndDT, " +
                " State = @State, " +
                " JobId = @JobId, " +
                " HostId = @HostId, " +
                " SaveDate = @SaveDate " +
                " WHERE RunId = @RunId";

            var param = new DynamicParameters();
            param.Add("RunId", run.RunId);
            param.Add("StartDT", run.StartDT);
            param.Add("EndDT", run.EndDT);
            param.Add("State", run.State);
            param.Add("JobId", run.JobId);
            param.Add("HostId", run.HostId);
            param.Add("SaveDate", run.SaveDate);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
    }
}