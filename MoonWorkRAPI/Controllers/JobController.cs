using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using System.Net;
using com.sun.corba.se.impl.orbutil.concurrent;
using System.Text;

namespace MoonWorkRAPI.Controllers
{

    [ApiController]
    [Route("v1/job")]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepo;

        public JobController(IJobRepository jobRepo)
        {
            _jobRepo = jobRepo;
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<List<JobModel>>> GetJobs()
        {
            try
            {
                var jobs = await _jobRepo.GetJobs();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{JobId}", Name = "JobByJobId")]
        public async Task<ActionResult<JobModel>> GetJob(int JobId)
        {
            try
            {
                var job = await _jobRepo.GetJob(JobId);
                if (job == null)
                    return NotFound();

                return Ok(job);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        // job + UserName select
        [HttpGet("joblist_username")]
        public async Task<ActionResult<List<JobUserNameModel>>> GetJobUserName()
        {
            try
            {
                var get = await _jobRepo.GetJobUserName();
                return Ok(get);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Running 중인 전체 job list 조회
        [HttpGet]
        [Route("running", Name = "R-J-053")]
        public async Task<ActionResult<List<JobModel>>> GetRunningJobs()
        {
            try
            {
                var runningJobs = await _jobRepo.GetRunningJobs();
                return Ok(runningJobs);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        //등록된 총 작업 개수
        [HttpGet]
        [Route("totalnum")]
        public object GetRegistNum()
        {
            try
            {
                var registnum = _jobRepo.GetRegistNum();
                return registnum;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // 오늘 추가된 작업 개수
        [HttpGet]
        [Route("addtoday")]
        public object GetAddToday()
        {
            try
            {
                var addtoday = _jobRepo.GetAddToday();
                return Ok(addtoday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //스케줄 등록이 안된 작업 개수
        [HttpGet]
        [Route("notregist")]
        public object GetNotRegist()
        {
            try
            {
                var notregist = _jobRepo.GetNotRegist();
                return Ok(notregist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 오늘 시작된 작업 개수
        [HttpGet]
        [Route("startschedule")]
        public object GetStartSchedule()
        {
            try
            {
                var startsche = _jobRepo.GetStartSchedule();
                return Ok(startsche);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 성공 작업 개수
        [HttpGet]
        [Route("success")]
        public object GetSuccess()
        {
            try
            {
                var success = _jobRepo.GetSuccess();
                return Ok(success);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 실패 작업 개수
        [HttpGet]
        [Route("failed")]
        public object GetFailed()
        {
            try
            {
                var failed = _jobRepo.GetFailed();
                return Ok(failed);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //job에 따른 유저네임, 스케쥴 정보 가져오기
        [HttpGet]
        [Route("{JobId}/GetJob_UserSchedule")]
        public ActionResult GetJob_UserSchedule(long JobId)
        {
            try
            {
                var allinfo = _jobRepo.GetJob_UserSchedule(JobId);
                if (allinfo == null) 
                    return NotFound();

                return Ok(allinfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //job에 따른 Host, Run 정보 가져오기
        [HttpGet]
        [Route("{JobId}/GetJob_HostRun")]
        public ActionResult GetJob_HostRun(long JobId)
        {
            try
            {
                var allinfo = _jobRepo.GetJob_HostRun(JobId);
                if (allinfo == null)
                    return NotFound();

                return Ok(allinfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //job에 대한 JobId, WorkflowName, schedule의 StartDT, EndDT
        [HttpGet("{JobId}/GetJob_WorkerFromMaster")]
        public ActionResult GetJob_WorkerFromMaster(long JobId)
        {
            try
            {
                var str = _jobRepo.GetJob_WorkerFromMaster(JobId);
                if (str == null)
                    return NotFound();

                return Ok(str);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // job의 동작중인 상태 확인
        [HttpGet("getstate")]
        public async Task<ActionResult<List<JobModel>>> GetJob_Status()
        {
            try
            {
                var stat = await _jobRepo.GetJob_State();
                return Ok(stat);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // job의 마지막 실행 시간
        [HttpGet("getlastrun")]
        public Object GetLastRun(long JobId)
        {
            var last = _jobRepo.GetLastRun(JobId);
            return last;
        }

        //job의 다음 실행 시간
        [HttpGet("getnextrun")]
        public Object GetNextRun(long JobId, string[] cron)
        {
            var next = _jobRepo.GetNextRun(JobId, cron);
            return next;
        }


        [HttpPost("create")]
        public Object CreateJob(JobModel job)
        {
            try
            {
                _jobRepo.CreateJob(job);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /*        [HttpPost("{JobId}/Schedule")]
                *//*        [Route("{JobId}/Schedule")]*//*
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
                }*/


        [HttpPut("update")]
        /*        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]*/
        /*        [Consumes(MediaTypeNames.Application.Json)]*/
        public ActionResult<JobModel> Update(JobModel job)
        {
            try
            {
                _jobRepo.UpdateJob(job);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

/*        [HttpPut("updateIsUse")]
        public ActionResult<JobModel> UpdateIsUse(JobModel job)
        {
            try
            {
                _jobRepo.UpdateIsUse(job);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }*/

        [HttpDelete("delete/{JobId}")]
        public IActionResult Delete(int JobId)
        {
            try
            {
                _jobRepo.DeleteJob(JobId);

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}