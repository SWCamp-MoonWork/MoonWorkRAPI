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

        //스케쥴 전체 SELECT
        [HttpGet]
        [Route("schedule")]
        public async Task<ActionResult<List<ScheduleModel>>> GetSchedules()
        {
            try
            {
                var schedule = await _ScheduleRepo.GetSchedules();
                return Ok(schedule);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // 모든 스케줄의 정보와 파일명
        [HttpGet("scheduleworkflowname")]
        public async Task<ActionResult<List<ScheduleWorkflowNameModel>>> GetSchedule_WorkflowName()
        {
            try
            {
                var schedule = await _ScheduleRepo.GetSchedule_WorkflowName();
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 가져오기
        [HttpGet]
        [Route("{JobId}/Schedule", Name = "ScheduleByJobId")]
        public async Task<ActionResult<ScheduleModel>> GetSchedule(long JobId)
        {
            try
            {
                var schedule = await _ScheduleRepo.GetSchedule(JobId);
                if (schedule == null)
                    return NotFound();

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 jobid에 대한 job의 job IsUse, Workflowname, schedule 의 모든 정보 가져오기
        [HttpGet("Schedule_IsUseSelect")]
        public async Task<ActionResult<List<Schedule_IsUseSelectModel>>> GetSchedule_IsUseSelect()
        {
            try
            {
                var schedule = await _ScheduleRepo.GetSchedule_IsUseSelect();
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 생성
        [HttpPost("{JobId}/Schedule")]
        /*        [Route("{JobId}/Schedule")]*/
        public ActionResult<ScheduleModel> Create(ScheduleModel schedule)
        {
            try
            {
                _ScheduleRepo.CreateSchedule(schedule);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 값 넣을때 jobid가 둘다 같아야 함
        // 특정 job에 대한 schedule 수정
        [HttpPut]
        [Route("{JobId}/Schedule")]
        public ActionResult<ScheduleModel> UpdateSchedule(ScheduleModel schedule)
        {
            try
            {
                _ScheduleRepo.UpdateSchedule(schedule);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 특정 job에 대한 schedule 삭제
        [HttpDelete]
        [Route("{JobId}/Schedule")]
        public IActionResult DeleteSchedule(long JobId)
        {
            try
            {
               _ScheduleRepo.DeleteSchedule(JobId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}