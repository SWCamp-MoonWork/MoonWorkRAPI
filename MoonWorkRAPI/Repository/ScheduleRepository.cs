using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{
    public interface IScheduleRepository
    {
        public Task <ScheduleModel> GetSchedule(int JobId);
        public Task CreateSchedule(ScheduleModel schedule);
        public Task UpdateSchedule(ScheduleModel schedule, int JobId);
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
        public async Task<ScheduleModel> GetSchedule(int JobId)
        {
            var query = "SELECT ScheduleId, JobId, ScheduleName, IsUse, ScheduleType, OneTimeOccurDT, ScheduleStartDT, ScheduleEndDT, SaveDate, UserId"
                + " from Schedule where JobId = @JobId";

            using (var connection = _context.CreateConnection())
            {
                var schedule = await connection.QuerySingleOrDefaultAsync<ScheduleModel>(query, new { JobId });
                return schedule;
            }
        }

        //특정 job에 대한 스케줄 등록
        public async Task CreateSchedule(ScheduleModel schedule)
        {
            var query = "INSERT INTO Schedule " +
                "(ScheduleId, SchaduleName, ScheduleType, ScheduleStartDT, ScheduleEndDT) " +
                "values(@ScheduleId, @ScheduleName, @ScheduleType, @ScheduleStartDT, @ScheduleEndDT)";

            var param = new DynamicParameters();
            param.Add("ScheduleId", schedule.ScheduleId);
            param.Add("ScheduleName", schedule.ScheduleName);
            param.Add("ScheduleType", schedule.ScheduleType);
            param.Add("ScheduleStartDT", schedule.ScheduleStartDT);
            param.Add("ScheduleEndDT", schedule.ScheduleEndDT);

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        //특정 job에 대한 스케줄 수정
        public async Task UpdateSchedule(ScheduleModel schedule, int JobId)
        {
            var query = "UPDATE Schedule SET " +
                "ScheduleName = @ScheduleName " +
                "ScheduleType = @ScheduleType " +
                "ScheduleStartDT = @ScheduleStartDT " +
                "ScheduleEndDT = @ScheduleEndDT";

            var param = new DynamicParameters();
            param.Add("ScheduleName", schedule.ScheduleName);
            param.Add("ScheduleType", schedule.ScheduleType);
            param.Add("ScheduleStartDT", schedule.ScheduleStartDT);
            param.Add("scheduleEndDT", schedule.ScheduleEndDT);

            using(var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, param);
            }
        }

        public async Task DeleteSchedule(int JobId)
        {
            var query = "DELETE FROM Schedule WHERE ScheduleId = @ScheduleId";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(query, new { JobId });
            }
        }
    }
}
