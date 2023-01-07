using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{

    //Job Repository Interface
    public interface IJobRepository
    {
        public Task<IEnumerable<JobModel>> GetJobs();
        public Task<JobModel> GetJob(int JobId);
        public Task CreateJob(JobModel job);
        public Task UpdateJob(int JobId, JobModel job);
        public Task DeleteJob(int JobId);
        public Task<IEnumerable<JobModel>> GetRunningJobs();
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
    }
}
