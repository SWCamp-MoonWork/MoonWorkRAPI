using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using System.Linq.Expressions;

namespace MoonWorkRAPI.Controllers
{
    [ApiController]
    [Route("v1/job")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _ScheduleRepo;
        public ScheduleController(IScheduleRepository ScheduleRepo)
        {
            _ScheduleRepo = ScheduleRepo;
        }

        // 특정 job에 대한 schedule 가져오기
        [HttpGet]
        [Route("{JobId}/Schedule", Name = "ScheduleByJobId")]
        public async Task<ActionResult<ScheduleModel>> GetSchedule(int JobId)
        {
            try
            {
            var schedule = await _ScheduleRepo.GetSchedule(JobId);
            if(schedule == null)
                    return NotFound();

                return Ok(schedule);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 생성
        [HttpPost]
        [Route("{JobId}/Schedule")]
        public async Task<ActionResult> CreateSchedule(int JobId)
        {
            try
            {
                ScheduleModel schedule = new ScheduleModel();

                schedule.ScheduleId = 100;
                schedule.ScheduleName = "test100";
                schedule.ScheduleType = false;
                schedule.ScheduleStartDT = DateTime.Now;
                schedule.ScheduleEndDT = DateTime.Now;

                await _ScheduleRepo.CreateSchedule(schedule);

                return Ok("Schedule 등록 성공!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 수정
        [HttpPut]
        [Route("{JobId}/Schedule")]
        public async Task<ActionResult> UpdateSchedule(int JobId)
        {
            try
            {
                var dbsche = await _ScheduleRepo.GetSchedule(JobId);
                if (dbsche == null)
                    return NotFound();
                await _ScheduleRepo.UpdateSchedule(dbsche, JobId);
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 삭제
        [HttpDelete]
        [Route("{JobId}/Schedule")]
        public async Task<ActionResult> DeleteSchedule(int JobId)
        {
            try
            {
                var dbsche = await _ScheduleRepo.GetSchedule(JobId);
                if (dbsche == null)
                    return NotFound();
                await _ScheduleRepo.DeleteSchedule(JobId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
