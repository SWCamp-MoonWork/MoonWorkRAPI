using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;

//
using System.Net;
using com.sun.corba.se.impl.orbutil.concurrent;
using System.Text;

//


namespace MoonWorkRAPI.Controllers
{

    //







    //


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
            catch(Exception ex)
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


        //job의 모든 정보 가져오기
        [HttpGet]
        [Route("{JobId}/GetJobAllInfo")]
        public ActionResult GetJobAllInfo(long JobId)
        {
            try
            {
                var allinfo =  _jobRepo.GetJobAllInfo(JobId);
                if (allinfo == null)
                    return NotFound();

                return Ok(allinfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // 추후엔 Spring에서 객체를 던져줘서 CreateJob(JobModel job) 으로 인자를 받아와야 할 것 같음.
        [HttpPost("create")]
        /*        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]*/
        /*        [Consumes(MediaTypeNames.Application.Json)]*/
        public ActionResult<JobModel> Create(JobModel job, byte[] Blob)
        {
            try
            {
                _jobRepo.CreateJob(job, Blob);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



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