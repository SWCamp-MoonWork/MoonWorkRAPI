using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IRunRepository
    {
        public Object CreateRun(RunModel run);
        public Task<RunModel> GetRunInfo(long RunId);
/*        public Task<RunModel> GetRun();*/
        public Task<IEnumerable<RunModel>> GetRunbyJobId(long JobId);
        public Task<IEnumerable<RunModel>> GetJob_RunRecord(long JobId);
        public Task<IEnumerable<RunModel>> GetJob_Duration(long JobId);
        public Task<IEnumerable<GraphModel>> GetGraph();
        public Task<IEnumerable<RunModel>> GetRunbyDate(DateTime FromDT, DateTime ToDT);
/*        public Task UpdateEndDT(RunModel run);*/
        public Task UpdateRun(RunModel run);
    }

    public class RunRepository : IRunRepository
    {
        private readonly DapperContext _context;

        public RunRepository(DapperContext context)
        {
            _context = context;
        }

        //run 시킨 후 run에 대한 JobId, WorkflowName 기록
        public object CreateRun(RunModel run)
        {
            var query = "INSERT INTO Run "
                + " (JobId, HostId, SaveDate) "
                + " VALUES "
                + " (@JobId, @HostId, SYSDATE())";

            var getrunid = "SELECT RunId " +
                " From Run " +
                " WHERE " +
                " JobId = @JobId " +
                " AND " +
                " HostId = @HostId " +
                " order by RunId desc limit 0,1";

            var param = new DynamicParameters();
            param.Add("RunId", run.RunId);
            /*param.Add("WorkflowName", run.WorkflowName);*/
            /*param.Add("StartDT", run.StartDT);
            param.Add("EndDT", run.EndDT);
            param.Add("State", run.State);*/
            param.Add("JobId", run.JobId);
            param.Add("HostId", run.HostId);
            param.Add("SaveDate", run.SaveDate);
            /*param.Add("ResultData", run.ResultData);*/

            using (var conn = _context.CreateConnection())
            {
                var str = conn.Execute(query, param);
                var runid = conn.QuerySingleOrDefault<RunModel>(getrunid, new {run.JobId, run.HostId});
                Console.WriteLine("str : " + str);
                Console.WriteLine("runid : " + runid);
                return runid;
            }
        }

        // 특정 runid에 대한 정보 가져오기
        public async Task<RunModel> GetRunInfo(long RunId)
        {
            var query = "SELECT RunId, WorkflowName, StartDT, EndDT, State, JobId, HostId, SaveDate, ResultData "
                + " FROM Run WHERE RunId = @RunId";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QuerySingleOrDefaultAsync<RunModel>(query, new { RunId });
                return run;
            }
        }

        //특정 JobId로 인해 발생한 run 조회
        public async Task<IEnumerable<RunModel>> GetRunbyJobId(long JobId)
        {
            var query = "SELECT RunId, WorkflowName, StartDT, EndDT, State, JobId, HostId, SaveDate, ResultData "
                + " FROM Run WHERE JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new { JobId });
                return run.ToList();
            }
        }

        //job의 run 기록 최근 5개
        public async Task<IEnumerable<RunModel>> GetJob_RunRecord(long JobId)
        {
            var query = "SELECT * FROM Run " +
                "WHERE JobId = @JobId " +
                "order by RunId desc " +
                "limit 0,5";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new { JobId });
                return run.ToList();
            }
        }

        //job의 run 기록 최근 20개 + duration
        public async Task<IEnumerable<RunModel>> GetJob_Duration(long JobId)
        {
            var query = "SELECT *, " +
                " (DATEDIFF(EndDT, StartDT) * 86400 + timediff(EndDT,StartDT)) as Duration " +
                " FROM Run " +
                " WHERE JobId = @JobId " +
                " order by RunId desc " +
                " limit 0,20";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new {JobId});

                return run.ToList();
            }
        }

        // 최근 1시간동안 실행했던 작업들의 run 시작시간, 듀레이션
        public async Task<IEnumerable<GraphModel>> GetGraph()
        {
            var query = "SELECT r.RunId, j.JobName, r.JobId, r.StartDT, " +
                " (DATEDIFF(r.EndDT, r.StartDT) * 86400 + timediff(r.EndDT,r.StartDT)) as Duration " +
                " From Run r, Job j " +
                " Where j.JobId = r.JobId AND r.StartDT>= DATE_ADD(NOW(), INTERVAL -1 HOUR) ";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<GraphModel>(query);
                return run.ToList();
            }
        }



        //일정 기간동안의 모든 run 조회
        public async Task<IEnumerable<RunModel>> GetRunbyDate(DateTime FromDT, DateTime ToDT)
        {

            var query = "SELECT RunId, WorkflowName, StartDT, EndDT, State, JobId, HostId, SaveDate "
                + " FROM Run WHERE StartDT >= @FromDT AND EndDT <= @ToDT";

            using (var connection = _context.CreateConnection())
            {
                var run = await connection.QueryAsync<RunModel>(query, new { FromDT, ToDT });
                return run.ToList();
            }
        }

        /*        // job run이 다 돌면 endDT update
                public async Task UpdateEndDT(RunModel run)
                {
                    var query = "INSERT INTO Run(EndDT) Values(@EndDT) " +
                        " WHERE RunId = @RunId";

                    var param = new DynamicParameters();
                    param.Add("RunId", run.RunId);
                    param.Add("EndDT", run.EndDT);

                    using(var conn = _context.CreateConnection())
                    {
                        await conn.ExecuteAsync(query, param);
                    }
                }*/

        // 특정 RunId에 대한 정보 업데이트
        public async Task UpdateRun(RunModel run)
        {
            var query = "UPDATE Run SET " +
                " WorkflowName = @WorkflowName, " +
                " StartDT = @StartDT, " +
                " EndDT = @EndDT, " +
                " State = @State, " +
                " SaveDate = SYSDATE(), " +
                " ResultData = @ResultData " +
                " WHERE RunId = @RunId";
/*                " JobId = @JobId, " +
                " HostId = @HostId, " +*/

            var param = new DynamicParameters();
            param.Add("WorkflowName", run.WorkflowName);
            param.Add("StartDT", run.StartDT);
            param.Add("EndDT", run.EndDT);
            param.Add("State", run.State);
            param.Add("ResultData", run.ResultData);
            param.Add("RunId", run.RunId);
            param.Add("SaveDate", run.SaveDate);
            /*            param.Add("JobId", run.JobId);
                        param.Add("HostId", run.HostId);*/

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
    }
}