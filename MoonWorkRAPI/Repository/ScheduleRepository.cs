using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IScheduleRepository
    {
        public Task<IEnumerable<ScheduleModel>> GetSchedule(int JobId);
/*        public Task<ScheduleModel> GetSche(long ScheduleId);*/
        public Task CreateSchedule(ScheduleModel schedule);
        public Task UpdateSchedule(ScheduleModel schedule);
        public Task DeleteSchedule(int JobId);
    }

    public class ScheduleRepository : IScheduleRepository
    {
        private readonly DapperContext _context;

        public ScheduleRepository(DapperContext context)
        {
            _context = context;
        }

        //특정 job에 대한 스케줄 가져오기
        public async Task<IEnumerable<ScheduleModel>> GetSchedule(int JobId)
        {
            var query = "SELECT ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, ScheduleStartDT, ScheduleEndDT, SaveDate, UserId"
                + " from Schedule where JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var schedule = await connection.QueryAsync<ScheduleModel>(query, new { JobId });
                return schedule.ToList();
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
                "  (@ScheduleId, @JobId, @ScheduleName, @IsUse, @ScheduleType, @OneTimeOccurDT, @CronExpression, @ScheduleStartDT, @ScheduleEndDT, @SaveDate, @UserId)";

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

        public async Task DeleteSchedule(int ScheduleId)
        {
            var query = "DELETE FROM Schedule WHERE ScheduleId = @ScheduleId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { ScheduleId });
            }
        }
    }
}