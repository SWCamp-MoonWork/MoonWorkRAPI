using Dapper;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{

    //Job Repository Interface
    public interface IJobRepository
    {
        public Task<IEnumerable<JobModel>> GetJobs();
        public Task<JobModel> GetJob(int JobId);
        public Task<IEnumerable<JobModel>> GetRunningJobs();
        public Task<JobModel> GetRegistNum();
        public Task<JobModel> GetAddToday();
        public Task<JobModel> GetNotRegist();
        public Task<JobModel> GetStartSchedule();
        public Task<JobModel> GetSuccess();
        public Task<JobModel> GetJobAllInfo();
        public Task CreateJob(JobModel job);
        public Task UpdateJob(int JobId, JobModel job);
        public Task DeleteJob(int JobId);

    }


    public class JobRepository : IJobRepository
    {
        private readonly DapperContext _context;
        public JobRepository(DapperContext context)
        {
            _context = context;
        }

        // 전체 Job 가져오기
        public async Task<IEnumerable<JobModel>> GetJobs()
        {
            var query = "SELECT JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId" +
                "   FROM Job";

            using (var connection = _context.CreateConnection())
            {
                var jobs = await connection.QueryAsync<JobModel>(query);
                return jobs.ToList();
            }
        }

        // 특정 Job 가져오기
        public async Task<JobModel> GetJob(int JobId)
        {
            var query = "SELECT JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId" +
                "   FROM Job" +
                "   WHERE JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var job = await connection.QuerySingleOrDefaultAsync<JobModel>(query, new { JobId });
                return job;
            }
        }

        //작동중인 job list
        public async Task<IEnumerable<JobModel>> GetRunningJobs()
        {
            var query = "SELECT JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId" +
                "   FROM Job" +
                "   WHERE IsUse = true";

            using (var connection = _context.CreateConnection())
            {
                var runningJobs = await connection.QueryAsync<JobModel>(query);
                return runningJobs.ToList();
            }
        }

        // 등록된 job 개수
        public async Task<JobModel> GetRegistNum()
        {
            var query = "SELECT COUNT(*) FROM Job";

            using (var connection = _context.CreateConnection())
            {
                var Regist = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return Regist;
            }
        }

        // 오늘 추가된 작업 개수
        public async Task<JobModel> GetAddToday()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE SaveDate = DATE(NOW())";
            using (var connection = _context.CreateConnection())
            {
                var add = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return add;
            }
        }

        // 스케줄 등록이 안된 작업 개수
        public async Task<JobModel> GetNotRegist()
        {
            var query = "SELECT COUNT(*) FROM Schedule WHERE JobId = null";
            using (var connection = _context.CreateConnection())
            {
                var notregist = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return notregist;
            }
        }

        //오늘 시작된 작업 개수
        public async Task<JobModel> GetStartSchedule()
        {
            var query = "SELECT COUNT(*) FROM Schedule WHERE SaveDate = DATE(NOW())";
            using (var connection = _context.CreateConnection())
            {
                var start = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return start;
            }
        }

        // 성공 실패 여부 작업 개수
        public async Task<JobModel> GetSuccess()
        {
            var query = "";
            using (var connection = _context.CreateConnection())
            {
                var success = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return success;
            }
        }



        // job의 모든 정보
        public async Task<JobModel> GetJobAllInfo()
        {
            var query = "SELECT j.JobId, j.JobName, j.WorkflowName, h.HostName, h.HostIP, h.IsUse, u.UserName, j.SaveDate, j.Note, s.ScheduleId, s.ScheduleName, "
                + " s.IsUse, s.ScheduleType, s.OneTimeOccurDT, s.ScheduleStartDT, s.ScheduleEndDT, s.SaveDate "
                + " From User u, Job j, Schedule s, Host h where u.UserId = j.UserId and h.HostId = u.UserId and j.JobId = s.ScheduleId";

            using (var connection = _context.CreateConnection())
            {
                var all = await connection.QuerySingleOrDefaultAsync<JobModel>(query);
                return all;
            }
        }

        // Job 생성하기
        public async Task CreateJob(JobModel job)
        {
            var query = "INSERT INTO Job" +
                "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId)" +
                "   VALUES" +
                "   (@JobId, @JobName, @IsUse, @WorkflowName, @WorkflowBlob, @Note, @SaveDate, @UserId)";

            var param = new DynamicParameters();
            param.Add("JobId", job.JobId);
            param.Add("JobName", job.JobName);
            param.Add("IsUse", job.IsUse);
            param.Add("WorkflowName", job.WorkflowName);
            param.Add("WorkflowBlob", job.WorkflowBlob);
            param.Add("Note", job.Note);
            param.Add("SaveDate", job.SaveDate);
            param.Add("UserId", job.UserId);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        public async Task UpdateJob(int JobId, JobModel job)
        {
            var query = "UPDATE Job SET" +
                "   JobName = @JobName," +
                "   IsUse = @IsUse," +
                "   WorkflowName = @WorkflowName," +
                "   WorkflowBlob = @WorkflowBlob," +
                "   Note = @Note," +
                "   SaveDate = @SaveDate," +
                "   UserId = @UserId" +
                "   WHERE JobId = @JobId";

            var param = new DynamicParameters();
            param.Add("JobId", JobId);
            param.Add("JobName", "UpdateTest"); //job.JobName 넣어야함
            param.Add("IsUse", job.IsUse);
            param.Add("WorkflowName", job.WorkflowName);
            param.Add("WorkflowBlob", job.WorkflowBlob);
            param.Add("Note", job.Note);
            param.Add("SaveDate", job.SaveDate);
            param.Add("UserId", job.UserId);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
        public async Task DeleteJob(int JobId)
        {
            var query = "DELETE FROM Job WHERE JobId = @JobId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { JobId });
            }
        }
    }
}