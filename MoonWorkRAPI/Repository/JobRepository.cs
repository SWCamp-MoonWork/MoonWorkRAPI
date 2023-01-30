using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static com.sun.net.httpserver.Authenticator;
using java.nio.charset;
using DocumentFormat.OpenXml.Office.Word;
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using sun.util.resources.cldr.sk;
using com.sun.org.apache.xpath.@internal.operations;
using java.rmi.server;
using System.Runtime.Intrinsics.Arm;
using javax.xml.bind;
using sun.misc;
using System.Text.Json.Serialization;
using System.Text.Json;
using DocumentFormat.OpenXml.Drawing;
using System.Net.Http.Json;

namespace MoonWorkRAPI.Repository
{

    //Job Repository Interface
    public interface IJobRepository
    {
        public Task<IEnumerable<JobModel>> GetJobs();
        public Task<JobModel> GetJob(int JobId);
        public Task<IEnumerable<JobUserNameModel>> GetJobUserName();
        public Task<IEnumerable<JobModel>> GetRunningJobs();
        /*        public Task<JobModel> GetRegistNum();*/
        public object GetRegistNum();
        public object GetAddToday();
        public object GetNotRegist();
        public object GetStartSchedule();
        public object GetSuccess();
        public object GetFailed();
        public Job_UserScheduleModel GetJob_UserSchedule(long JobId);
        public Job_HostRunModel GetJob_HostRun(long JobId);
        public void CreateJob(JobModel job);
        public Task UpdateJob(JobModel job);
        public Task UpdateIsUse(JobModel job);
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
            

            var query = "SELECT JobId, JobName, IsUse, WorkflowName, CONVERT(WorkflowBlob using utf8) as WorkflowBlob, Note, SaveDate, UserId" +
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
            var query = "SELECT JobId, JobName, IsUse, WorkflowName, CONVERT(WorkflowBlob using utf8) as WorkflowBlob, Note, SaveDate, UserId" +
                "   FROM Job" +
                "   WHERE JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var job = await connection.QuerySingleOrDefaultAsync<JobModel>(query, new { JobId });
                return job;
            }
        }

        // job + UserName select
        public async Task<IEnumerable<JobUserNameModel>> GetJobUserName()
        {
            var query = "SELECT j.JobId, j.JobName, j.IsUse, j.WorkflowName, " +
                " CONVERT(j.WorkflowBlob using utf8) as WorkflowBlob, j.Note, j.SaveDate, j.UserId, u.UserName " +
                " from Job j, User u WHERE u.UserId = j.UserId";

            using(var connection = _context.CreateConnection())
            {
                var job = await connection.QueryAsync<JobUserNameModel>(query);
                return job.ToList();
            }
        }

        //작동중인 job list
        public async Task<IEnumerable<JobModel>> GetRunningJobs()
        {
            var query = "SELECT JobId, JobName, IsUse, WorkflowName, CONVERT(WorkflowBlob using utf8) as WorkflowBlob, Note, SaveDate, UserId" +
                "   FROM Job" +
                "   WHERE IsUse = true";

            using (var connection = _context.CreateConnection())
            {
                var runningJobs = await connection.QueryAsync<JobModel>(query);
                Console.Write(runningJobs);
                return runningJobs.ToList();
            }
        }

        // 등록된 job 개수
        public object GetRegistNum()
        {
            var query = "SELECT COUNT(*) FROM Job";

            using (var connection = _context.CreateConnection())
            {
                var Regist = connection.QueryFirstOrDefault(query);
                Console.Write(Regist);
                return Regist;

            }
        }


        // 오늘 추가된 작업 개수
        public object GetAddToday()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE SaveDate = DATE(NOW())";
            using (var connection = _context.CreateConnection())
            {
                var add = connection.QuerySingleOrDefault(query);
                return add;
            }
        }

        // 스케줄 등록이 안된 작업 개수
        public object GetNotRegist()
        {
            var query = "SELECT COUNT(*) FROM Job j left outer join Schedule s on j.JobId = s.JobId where s.ScheduleId is null";
            using (var connection = _context.CreateConnection())
            {
                var notregist = connection.QuerySingleOrDefault(query);
                return notregist;
            }
        }

        //오늘 시작된 작업 개수
        public object GetStartSchedule()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE DATE_FORMAT(SaveDate, \"%Y-%m-%d\") = CURDATE()";
            using (var connection = _context.CreateConnection())
            {
                var start = connection.QuerySingleOrDefault(query);
                return start;
            }
        }

        // 성공 실패 여부 작업 개수
        public object GetSuccess()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE Status = 'success'";
            using (var connection = _context.CreateConnection())
            {
                var success = connection.QuerySingleOrDefault(query);
                return success;
            }
        }

        // 성공 실패 여부 작업 개수
        public object GetFailed()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE Status = 'failed'";

            using (var connection = _context.CreateConnection())
            {
                var failed = connection.QuerySingleOrDefault(query);
                return failed;
            }
        }


        // job에 대한 유저, 스케줄 정보
        public Job_UserScheduleModel GetJob_UserSchedule(long JobId)
        {
            var query = "SELECT j.JobId, j.JobName, j.IsUse as JobIsUse, j.WorkflowName, j.SaveDate as JobSaveDate, j.Note as JobNote, " +
                " u.UserName, " +
                " s.ScheduleId, s.ScheduleName, s.IsUse as ScheduleIsUse, s.ScheduleType, s.OneTimeOccurDT, s.CronExpression, s.ScheduleStartDT, s.ScheduleEndDT, s.SaveDate as ScheduleSaveDate" +
                " from Job j  " +
                " left outer join Schedule s on j.JobId = s.JobId" +
                " left outer join User u on j.UserId = u.UserId" +
                " where j.JobId = @JobId;";

            using (var connection = _context.CreateConnection())
            {
                var all = connection.QuerySingleOrDefault<Job_UserScheduleModel>(query, new { JobId });
                return all;
            }
        }

        // job에 대한 호스트, 런 정보
        public Job_HostRunModel GetJob_HostRun(long JobId)
        {
            var query = "SELECT h.HostName, h.HostIp, r.StartDT, r.EndDT, r.State, r.SaveDate " +
                " from Host h " +
                " left outer join Run r on h.HostId = r.HostId " +
                " where r.JobId = @JobId  " +
                " order by r.StartDT desc limit 1";

            using (var connection = _context.CreateConnection())
            {
                var all = connection.QuerySingleOrDefault<Job_HostRunModel>(query, new { JobId });
                return all;
            }
        }

        //job 생성
        public void CreateJob(JobModel job)
        {
            string test = job.WorkflowBlob;
            byte[] ba = System.Text.Encoding.Default.GetBytes(test);

            /*for(int i = 0; i < ba.Length; i++)
            {
                Console.WriteLine(ba[i]);
            }*/


            var query1 = "INSERT INTO Job " +
                "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId) " +
                "   VALUES " +
                "   (@JobId, @JobName, true, @WorkflowName, @WorkflowBlob, @Note, SYSDATE(), @UserId) ";

            var query2 = "Update Job SET " +
                "   WorkflowBlob = @WorkflowBlob WHERE JobId = @JobId";

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
                conn.Execute(query1, param);
            }
        }

        public async Task UpdateJob(JobModel job)
        {

            string test = job.WorkflowBlob;
            byte[] ba = System.Text.Encoding.Default.GetBytes(test);

            var query = "UPDATE Job SET" +
                "   JobName = @JobName," +
/*                "   IsUse = @IsUse," +*/
                "   WorkflowName = @WorkflowName," +
                "   WorkflowBlob = @WorkflowBlob," +
                "   Note = @Note," +
                "   SaveDate = SYSDATE()" +
/*                "   UserId = @UserId" +*/
                "   WHERE JobId = @JobId";

            var param = new DynamicParameters();
            param.Add("JobId", job.JobId);
            param.Add("JobName", job.JobName);
/*            param.Add("IsUse", job.IsUse);*/
            param.Add("WorkflowName", job.WorkflowName);
            param.Add("WorkflowBlob", job.WorkflowBlob);
            param.Add("Note", job.Note);
            param.Add("SaveDate", job.SaveDate);
/*            param.Add("UserId", job.UserId);*/

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }
        // Job change IsUse
        public async Task UpdateIsUse(JobModel job)
        {
            var query = "Update Job Set IsUse = @IsUse where JobId = @JobId";

            var param = new DynamicParameters();
            param.Add("JobId", job.JobId);
            param.Add("IsUse", job.IsUse);

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