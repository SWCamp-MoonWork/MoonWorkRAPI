using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IScheduleRepository
    {
        public Task<IEnumerable<ScheduleModel>> GetSchedules();
        public Task<IEnumerable<ScheduleWorkflowNameModel>> GetSchedule_WorkflowName();
        public Task<ScheduleModel> GetSchedule(long JobId);
        public Task<IEnumerable<Schedule_IsUseSelectModel>> GetSchedule_IsUseSelect();
/*        public Task<ScheduleModel> GetSche(long ScheduleId);*/
        public Task CreateSchedule(ScheduleModel schedule);
        public Task UpdateSchedule(ScheduleModel schedule);
        public Task DeleteSchedule(long JobId);
    }

    public class ScheduleRepository : IScheduleRepository
    {
        private readonly DapperContext _context;

        public ScheduleRepository(DapperContext context)
        {
            _context = context;
        }

        //전체 스케쥴 가져오기
        public async Task<IEnumerable<ScheduleModel>> GetSchedules()
        {
            var query = "SELECT ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, CronExpression, ScheduleStartDT, " +
                " ScheduleEndDT, SaveDate, UserId from Schedule;";

            using (var connection = _context.CreateConnection())
            {
                var schedules = await connection.QueryAsync<ScheduleModel>(query);
                return schedules.ToList();
            }
        }

        // 모든 스케줄의 정보와 파일명 가져오기
        public async Task<IEnumerable<ScheduleWorkflowNameModel>> GetSchedule_WorkflowName()
        {
            var query = "SELECT s.*, j.WorkflowName from Job j, Schedule s where j.JobId = s.JobId";

            using(var connection = _context.CreateConnection())
            {
                var schedule = await connection.QueryAsync<ScheduleWorkflowNameModel>(query);
                return schedule.ToList();
            }
        }
        //특정 job에 대한 스케줄 가져오기
        public async Task<ScheduleModel> GetSchedule(long JobId)
        {
            var query = "SELECT ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, ScheduleStartDT, ScheduleEndDT, SaveDate, UserId"
                + " from Schedule where JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var schedule = await connection.QuerySingleOrDefaultAsync<ScheduleModel>(query, new { JobId });
                return schedule;
            }
        }

        //job, schedule에서 IsUse가 둘다 1인 row
        public async Task<IEnumerable<Schedule_IsUseSelectModel>> GetSchedule_IsUseSelect()
        {
            var query = "SELECT j.IsUse as JobIsUse, j.WorkflowName, s.ScheduleId, s.JobId, s.ScheduleName, s.IsUse, " +
                " s.ScheduleType, s.OneTimeOccurDT, s.CronExpression, " +
                " s.ScheduleStartDT, s.ScheduleEndDT, s.SaveDate, s.UserId " +
                " from Job j, Schedule s " +
                " WHERE j.IsUse = true and s.IsUse = true and j.JobId = s.JobId";

            using (var connection = _context.CreateConnection())
            {
                var SelectIsUse = await connection.QueryAsync<Schedule_IsUseSelectModel>(query);
                return SelectIsUse.ToList();
            }
        }

/*        // 특정 schedule 가져오기
        public async Task<ScheduleModel> GetSche(long ScheduleId)
        {
            var query = "SELECT ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, ScheduleStartDT, ScheduleEndDT, SaveDate, UserId"
                + " From Schedule WHERE ScheduleId = @ScheduleId";

            using (var connection = _context.CreateConnection())
            {
                var sche = await connection.QuerySingleOrDefaultAsync<ScheduleModel>(query, new { ScheduleId });
                return sche;
            }
        }*/

        //입력할때 JobId 2개가 같아야 값이 들어감
        //특정 job에 대한 스케줄 등록
        public async Task CreateSchedule(ScheduleModel schedule)
        {
            var query = "INSERT INTO Schedule " +
                "  (ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, CronExpression,  ScheduleStartDT, ScheduleEndDT, SaveDate, UserId) " +
                "  VALUES" +
                "  (@ScheduleId, @JobId, @ScheduleName, true, @ScheduleType, @OneTimeOccurDT, @CronExpression, @ScheduleStartDT, @ScheduleEndDT, @SaveDate, @UserId)";

            var param = new DynamicParameters();
            param.Add("ScheduleId", schedule.ScheduleId);
            param.Add("JobId", schedule.JobId);
            param.Add("ScheduleName", schedule.ScheduleName);
            param.Add("IsUse", schedule.IsUse);
            param.Add("ScheduleType", schedule.ScheduleType);
            param.Add("OneTimeOccurDT", schedule.OneTimeOccurDT);
            param.Add("CronExpression", schedule.CronExpression);
            param.Add("ScheduleStartDT", schedule.ScheduleStartDT);
            param.Add("ScheduleEndDT", schedule.ScheduleEndDT);
            param.Add("SaveDate", schedule.SaveDate);
            param.Add("UserId", schedule.UserId);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        //특정 job에 대한 스케줄 수정
        public async Task UpdateSchedule(ScheduleModel schedule)
        {
            var query = "UPDATE Schedule SET " +
                " ScheduleId = @ScheduleId, " +
                " JobId = @JobId, " +
                " ScheduleName = @ScheduleName, " +
                " IsUse = @IsUse, " +
                " ScheduleType = @ScheduleType, " +
                " OneTimeOccurDT = @OneTimeOccurDT, " +
                " CronExpression = @CronExpression, " +
                " ScheduleStartDT = @ScheduleStartDT, " +
                " ScheduleEndDT = @ScheduleEndDT, " +
                " SaveDate = @SaveDate, " +
                " UserId = @UserId " +
                " WHERE ScheduleId = @ScheduleId";

            var param = new DynamicParameters();
            param.Add("ScheduleId", schedule.ScheduleId);
            param.Add("JobId", schedule.JobId);
            param.Add("ScheduleName", schedule.ScheduleName);
            param.Add("IsUse", schedule.IsUse);
            param.Add("ScheduleType", schedule.ScheduleType);
            param.Add("OneTimeOccurDT", schedule.OneTimeOccurDT);
            param.Add("CronExpression", schedule.CronExpression);
            param.Add("ScheduleStartDT", schedule.ScheduleStartDT);
            param.Add("ScheduleEndDT", schedule.ScheduleEndDT);
            param.Add("SaveDate", schedule.SaveDate);
            param.Add("UserId", schedule.UserId);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        //스케줄 삭제
        public async Task DeleteSchedule(long ScheduleId)
        {
            var query = "DELETE FROM Schedule WHERE ScheduleId = @ScheduleId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { ScheduleId });
            }
        }
    }
}