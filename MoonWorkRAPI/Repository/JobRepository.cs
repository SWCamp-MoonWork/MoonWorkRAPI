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
using DocumentFormat.OpenXml;
using Quartz;
using System.Threading.Tasks;
using com.sun.istack.@internal;
using Quartz.Impl;
using DocumentFormat.OpenXml.Spreadsheet;
using String = com.sun.org.apache.xpath.@internal.operations.String;
using static Quartz.Logging.OperationName;

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
        public Job_UserScheduleModel GetJob_WorkerFromMaster(long JobId);
        public Task<IEnumerable<JobModel>> GetJob_State();
        public Object GetLastRun(long JobId);
        public Object GetNextRun(long JobId);
        public object GetRunningJobCount();
        public object GetUsingJobCount();
        public void CreateJob(JobModel job);
        public Task UpdateJob(JobModel job);
        public Task UpdateJob_State1(long JobId);
        public Task UpdateJob_State0(long JobId);
/*        public Task UpdateIsUse(JobModel job);*/
        public void DeleteJob(int JobId);
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
                " CONVERT(j.WorkflowBlob using utf8) as WorkflowBlob, j.Note, j.SaveDate, j.UserId, u.Name " +
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
            var query = "SELECT COUNT(*) FROM Job WHERE DATE_FORMAT(SaveDate, \"%Y-%m-%d\") = CURDATE()";
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
            var query = "SELECT COUNT(*) FROM Schedule s, Job j WHERE DATE_FORMAT(s.ScheduleStartDT, \"%Y-%m-%d\") = CURDATE() and j.JobId = s.JobId";
            using (var connection = _context.CreateConnection())
            {
                var start = connection.QuerySingleOrDefault(query);
                return start;
            }
        }

        // 작동 여부 작업 개수 - 실행중
        public object GetSuccess()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE State = 01";
            using (var connection = _context.CreateConnection())
            {
                var success = connection.QuerySingleOrDefault(query);
                return success;
            }
        }

        // 작동 여부 작업 개수 - 정지
        public object GetFailed()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE State = 00";

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

        //job에 대한 JobId, WorkflowName, schedule의 StartDT, EndDT
        public Job_UserScheduleModel GetJob_WorkerFromMaster(long JobId)
        {
            var query = "SELECT j.JobId, j.WorkflowName, s.ScheduleStartDT, s.ScheduleEndDT FROM Job j, Schedule s WHERE j.JobId = @JobId and j.JobId = s.JobId";

            using (var connection = _context.CreateConnection())
            {
                var str = connection.QuerySingleOrDefault<Job_UserScheduleModel>(query, new {JobId});
                return str;
            }
        }

        //job의 동작중인 상태 확인
        public async Task<IEnumerable<JobModel>> GetJob_State()
        {
            var query = "SELECT JobId, State FROM Job";

            using (var connection = _context.CreateConnection())
            {
                var str = await connection.QueryAsync<JobModel>(query);
                return str.ToList();
            }
        }

        // 마지막에 실행된 job의 시간 구하기
        public Object GetLastRun(long JobId)
        {
            var lastrun = "SELECT convert(EndDT,datetime(3)) FROM Run " +
                " Where EndDT is not null order by RunId desc limit 1";

            using (var conn = _context.CreateConnection())
            {
                var str = conn.QuerySingleOrDefault<Object>(lastrun, new { JobId });
                return str;
            }
        }

        //다음에 실행될 job의 시간 구하기
        public Object GetNextRun(long JobId)
        {
            // jobid에 따른 크론식 가져오기
            var getcron = "SELECT s.CronExpression FROM Job j, Schedule s " +
                "WHERE j.JobId = s.JobId and j.JobId = @JobId";


            // jobid에 따른 최근의 실행날짜 가져오기
            var laststartrun = "SELECT r.StartDT FROM Run r, Job j " +
                "Where j.JobId = @JobId and j.JobId = r.jobId " +
                "order by RunId desc limit 1";

            using (var conn = _context.CreateConnection())
            {
                var str = conn.QuerySingleOrDefault<string>(getcron, new { JobId });
                var start = conn.QuerySingleOrDefault<DateTime>(laststartrun, new { JobId });
                var expression = new CronExpression(str);
                DateTimeOffset? time = expression.GetTimeAfter(start);
                return time;
            }
        }

        // 작동중인 job 카운트
        public object GetRunningJobCount()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE State = '01'";

            using(var conn = _context.CreateConnection())
            {
                var count = conn.QuerySingleOrDefault(query);
                return count;
            }
        }


        // 활성화된 job 카운트
        public object GetUsingJobCount()
        {
            var query = "SELECT COUNT(*) FROM Job WHERE IsUse = true";

            using(var conn = _context.CreateConnection())
            {
                var count = conn.QuerySingleOrDefault(query);
                return count;
            }
        }


        //job 생성
        public void CreateJob(JobModel job)
        {

            var insert = "INSERT INTO Job " +
                "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId) " +
                "   VALUES " +
                "   (@JobId, @JobName, true, @WorkflowName, @WorkflowBlob, @Note, SYSDATE(), @UserId) ";


            var param = new DynamicParameters();
/*            param.Add("JobId", job.JobId);*/
            param.Add("JobName", job.JobName);
            param.Add("IsUse", job.IsUse);
            param.Add("WorkflowName", job.WorkflowName);
            param.Add("WorkflowBlob", job.WorkflowBlob);
            param.Add("Note", job.Note);
            param.Add("SaveDate", job.SaveDate);
            param.Add("UserId", job.UserId);

            using (var conn = _context.CreateConnection())
            {
                conn.Execute(insert, param);
            }
        }

        public async Task UpdateJob(JobModel job)
        {

            var query = "UPDATE Job SET" +
                "   JobName = @JobName," +
                "   IsUse = @IsUse," +
                "   WorkflowName = @WorkflowName," +
                "   WorkflowBlob = @WorkflowBlob," +
                "   Note = @Note," +
                "   SaveDate = SYSDATE()" +
/*                "   UserId = @UserId" +*/
                "   WHERE JobId = @JobId";

            var param = new DynamicParameters();
            param.Add("JobId", job.JobId);
            param.Add("JobName", job.JobName);
            param.Add("IsUse", job.IsUse);
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

        // job의 state를 1로 업데이트
        public async Task UpdateJob_State1(long JobId)
        {
            var query = "UPDATE Job SET " +
                " State = 01 " +
                " WHERE JobId = @JobId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { JobId });
            }
        }

        //job의 state를 0으로 업데이트
        public async Task UpdateJob_State0(long JobId)
        {
            var query = "UPDATE Job SET " +
                " State = 00 " +
                " WHERE JobId = @JobId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { JobId });
            }
        }

        // job 삭제
        public void DeleteJob(int JobId)
        {
            var query = "DELETE FROM Job WHERE JobId = @JobId";

            using (var conn = _context.CreateConnection())
            {
                conn.Execute(query, new { JobId });
            }
        }
    }
}