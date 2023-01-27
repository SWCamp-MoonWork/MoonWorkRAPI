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

namespace MoonWorkRAPI.Repository
{

    //Job Repository Interface
    public interface IJobRepository
    {
        public Task<IEnumerable<JobModel>> GetJobs();
        public Task<JobModel> GetJob(int JobId);
        public Task<IEnumerable<JobModel>> GetRunningJobs();
/*        public Task<JobModel> GetRegistNum();*/
        public object GetRegistNum();
        public object GetAddToday();
        public object GetNotRegist();
        public object GetStartSchedule();
        public object GetSuccess();
        public object GetFailed();
        public JobAllInfoModel GetJobAllInfo(long JobId);
        public void CreateJob(JobModel job);
        public Task UpdateJob(JobModel job);
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


        // job의 모든 정보
        public JobAllInfoModel GetJobAllInfo(long JobId)
        {
            var query = "SELECT j.JobId, j.JobName, j.WorkflowName, h.HostName, h.HostIP, h.IsUse, u.UserName, j.SaveDate, j.Note, s.ScheduleId, s.ScheduleName, "
                + " s.IsUse, s.ScheduleType, s.OneTimeOccurDT, s.ScheduleStartDT, s.ScheduleEndDT, s.SaveDate "
                + " from Job j left outer join (Schedule s, Host h, Run r, User u) on "
                + " (u.UserId = j.UserId and j.JobId = s.JobId and j.JobId = r.JobId and h.HostId = r.HostId) where j.JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var all = connection.QuerySingleOrDefault<JobAllInfoModel>(query, new { JobId });
                return all;
            }
        }

        //job 생성
        public void CreateJob(JobModel job)
        {
            Console.WriteLine(" hello ");
            /*string filePath = "job.WorkflowBlob";*/
            var query1 = "INSERT INTO Job " +
                "   (JobId, JobName, IsUse, WorkflowName, Note, SaveDate, UserId) " +
                "   VALUES " +
                "   (@JobId, @JobName, @IsUse, @WorkflowName, @Note, @SaveDate, @UserId) ";

            var query2 = "Update Job SET " +
                "WorkflowBlob = @WorkflowBlob WHERE JobId = @JobId";

            FileStream stream = new FileStream("job.WorkflowBlob", FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] blob = reader.ReadBytes((int)stream.Length);

            SqlParameter par = new SqlParameter("@Job.WorkflowBlob", SqlDbType.NChar, blob.Length);
            par.Value = blob;

            Console.WriteLine(par);

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
                conn.Execute(query2, par);
            }
        }


        /*        public async Task CreateJob(JobModel job)
                {
                    string filePath = @"job.WorkflowBlob";
                    byte[] bytes = Encoding.UTF8.GetBytes(filePath);
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    BinaryReader reader = new BinaryReader(fs);
                    Console.WriteLine(" Hello ");
                    byte[] BlobValue = reader.ReadBytes((int)fs.Length);

                    fs.Close();
                    reader.Close();

                    var param = new DynamicParameters();
                    param.Add("JobId", job.JobId);
                    param.Add("JobName", job.JobName);
                    param.Add("IsUse", job.IsUse);
                    param.Add("WorkflowName", job.WorkflowName);
                    param.Add("WorkflowBlob", job.WorkflowBlob);
                    param.Add("Note", job.Note);
                    param.Add("SaveDate", job.SaveDate);
                    param.Add("UserId", job.UserId);
                    using (var connect = _context.CreateConnection())
                    {
                        SqlConnection BlobsDataBaseConn = new SqlConnection("Server=rds-moonwork.co3bxrfx8ip8.ap-northeast-2.rds.amazonaws.com; Database = Moonwork; Uid = rdsmw; Pwd = dkfeldptm;Allow User Variables=True");
                        SqlCommand SaveBlobeCommand = new SqlCommand();
                        SaveBlobeCommand.Connection = BlobsDataBaseConn;
                        SaveBlobeCommand.CommandType = CommandType.Text;
                        SaveBlobeCommand.CommandText = "INSERT INTO Job" +
                       "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId)" +
                       "   VALUES" +
                       "   (@JobId, @JobName, @IsUse, @WorkflowName, @WorkflowBlob, @Note, @SaveDate, @UserId)";

                        SqlParameter BlobFileNameParam = new SqlParameter("@WorkflowBlob", SqlDbType.NChar);
                        SqlParameter BlobFileParam = new SqlParameter("@WorkflowBlob", SqlDbType.Binary);
                        SaveBlobeCommand.Parameters.Add(BlobFileParam);
                        BlobFileParam.Value = BlobValue;

                        try
                        {
                            SaveBlobeCommand.Connection.Open();
                            SaveBlobeCommand.ExecuteNonQuery();
                            Console.Write("Hello World");
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex);
                            Console.Write("Error");
                        }
                        finally
                        {
                            SaveBlobeCommand.Connection.Close();
                        }

                        await connect.ExecuteAsync(SaveBlobeCommand.CommandText, param);
                    }
                }*/



        /*        public async Task CreateJob(JobModel job)
                {
                    *//*            var query = "INSERT INTO Job" +
                                    "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId)" +
                                    "   VALUES" +
                                    "   (@JobId, @JobName, @IsUse, @WorkflowName, @WorkflowBlob, @Note, @SaveDate, @UserId)";*//*


                    string SQL;
                    UInt32 FileSize;
                    byte[] blob;
                    FileStream fs;

                    string ConString = "Server=rds-moonwork.co3bxrfx8ip8.ap-northeast-2.rds.amazonaws.com; Database = Moonwork; Uid = rdsmw; Pwd = dkfeldptm;Allow User Variables=True";

                    try
                    {
                        MySqlConnection con = new MySqlConnection(ConString);
                        MySqlCommand cmd = new MySqlCommand();
                        BinaryReader br;


                        fs = new FileStream(blob, FileMode.Open, FileAccess.Read);
                        br = new BinaryReader(fs);
                        FileSize = (UInt32)fs.Length;

                        *//*blob = new byte[FileSize];*//*
                        blob = br.ReadBytes((int)fs.Length);
                        fs.Read(blob, 0, (int)FileSize);
                        fs.Close();

                        con.Open();

                        SQL = "INSERT INTO Job" +
                        "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId)" +
                        "   VALUES" +
                        "   (@JobId, @JobName, @IsUse, @WorkflowName, @WorkflowBlob, @Note, @SaveDate, @UserId)";

                        cmd.Connection = con;
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddWithValue("@JobId", job.JobId);
                        cmd.Parameters.AddWithValue("@JobName", job.JobName);
                        cmd.Parameters.AddWithValue("@IsUse", job.IsUse);
                        cmd.Parameters.AddWithValue("@WorkflowName", job.WorkflowName);
                        cmd.Parameters.AddWithValue("@WorkflowBlob", job.WorkflowBlob);
                        cmd.Parameters.AddWithValue("@Note", job.Note);
                        cmd.Parameters.AddWithValue("@SaveDate", job.SaveDate);
                        cmd.Parameters.AddWithValue("@UserId", job.UserId);

                        cmd.ExecuteNonQuery();

                        Console.Write("success!!");

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
                            await conn.ExecuteAsync(SQL, cmd);
                        }

                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.Write("failed");
                    }
                }*/

        /*        private byte[] GetImageByte(string filePath)
                {
                    string connectionStr = "Server=rds-moonwork.co3bxrfx8ip8.ap-northeast-2.rds.amazonaws.com; Database = Moonwork; Uid = rdsmw; Pwd = dkfeldptm;Allow User Variables=True;";
                    SqlConnection conn;
                    FileStream fs;
                    BinaryReader br;
                    BinaryWriter bw;
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);

                    byte[] imageBytes = br.ReadBytes((int)fs.Length);

                    br.Close();
                    fs.Close();

                    return imageBytes;
                }*/

        /*        public void Insert(JobModel job, string fileName)
                {
                    String ConnectionStr = "Server=rds-moonwork.co3bxrfx8ip8.ap-northeast-2.rds.amazonaws.com; Database = Moonwork; Uid = rdsmw; Pwd = dkfeldptm;Allow User Variables=True;";
                    SqlConnection conn = new SqlConnection(ConnectionStr);
                    conn = new SqlConnection(ConnectionStr);
                    byte[] imageBytes = this.GetImageByte(fileName);
                    string sql = "INSERT INTO Job " +
                        " (WorkflowBlob) " +
                        " VALUES " +
                        " (@WorkflowBlob) " +
                        " WHERE JobId = @JobId ";

                    SqlCommand myCommand = new SqlCommand(sql, conn);
                    SqlParameter parm = new SqlParameter("@WorkflowBlob", SqlDbType.Image, imageBytes.Length);
                    parm.Value = imageBytes;
                    myCommand.Parameters.Add(parm);
                }*/

        /*        public async Task CreateJob(JobModel job, byte[] fileName)
                {
        *//*            byte[] ba = Encoding.UTF8.GetBytes(fileName);
                    Console.WriteLine(ba);
                    Console.WriteLine("{0}", ba);
                    Console.WriteLine("{0}", System.Text.Encoding.Default.GetString(ba));*//*

                    var query = "INSERT INTO Job" +
                        "   (JobId, JobName, IsUse, WorkflowName, WorkflowBlob, Note, SaveDate, UserId)" +
                        "   VALUES" +
                        "   (@JobId, @JobName, @IsUse, @WorkflowName, @WorkflowBlob, @Note, @SaveDate, @UserId)";

                    String ConnectionStr = "Server=rds-moonwork.co3bxrfx8ip8.ap-northeast-2.rds.amazonaws.com; Database = Moonwork; Uid = rdsmw; Pwd = dkfeldptm;Allow User Variables=True;";
                    SqlConnection conn = new SqlConnection(ConnectionStr);
                    conn.Open();
                    SqlCommand MyCommand = new SqlCommand("INSERT INTO Job" +
                        "   (JobId, JobName, IsUse, WorkflowName, Note, SaveDate, UserId)" +
                        "   VALUES" +
                        "   (@JobId, @JobName, @IsUse, @WorkflowName,  @Note, @SaveDate, @UserId)", conn);
                    SqlDataReader myReader = MyCommand.ExecuteReader(CommandBehavior.SequentialAccess);

        *//*            FileStream fs;
                    BinaryReader br;
                    BinaryWriter bw;

                    int bufferSize = 100;
                    byte[] outbyte = new byte[bufferSize];
                    long retval;
                    long startIndex;*//*

                    var param = new DynamicParameters();
                    param.Add("JobId", job.JobId);
                    param.Add("JobName", job.JobName);
                    param.Add("IsUse", job.IsUse);
                    param.Add("WorkflowName", job.WorkflowName);
                    param.Add("WorkflowBlob", job.WorkflowBlob);
                    param.Add("Note", job.Note);
                    param.Add("SaveDate", job.SaveDate);
                    param.Add("UserId", job.UserId);

                    conn = new SqlConnection(ConnectionStr);
                    byte[] ba = Encoding.UTF8.GetBytes(fileName);
                    byte[] imageBytes = this.GetImageByte(ba);
                    Stream stream = new FileStream("WorkflowBlob", FileMode.OpenOrCreate);
                    using (BinaryWriter wr = new BinaryWriter(stream))
                    {
                        wr.Write(imageBytes);
                    }

                    string sql = "INSERT INTO Job " +
                        " (WorkflowBlob) " +
                        " VALUES " +
                        " (@WorkflowBlob) ";
        *//*            " WHERE JobId = @JobId ";*//*

                    SqlCommand myCommand = new SqlCommand(sql, conn);
                    SqlParameter WorkflowBlob = new SqlParameter("@WorkflowBlob", SqlDbType.Image, imageBytes.Length);
                    WorkflowBlob.Value = imageBytes;
                    myCommand.Parameters.Add(WorkflowBlob);

                    conn.Open();
                    myCommand.ExecuteNonQuery();
                    conn.Close();


                    using (var connec = _context.CreateConnection())
                    {
                        await connec.ExecuteAsync(query, param);
                    }

                    myReader.Close();
                    conn.Close();
                }*/


        public async Task UpdateJob(JobModel job)
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