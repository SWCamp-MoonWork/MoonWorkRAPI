using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;

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

        /*
         * # 비동기(Asynchronous)
         * 서버에게 데이터를 요청한 후 요청에 따른 응답을 계속 기다리지 않아도되며 
         * 다른 외부 활동을 수행하여도되고 서버에게 다른 요청사항을 보내도 상관없다.
         * 
         * public async Task<ActionResult<List<MemberModel>>> Get() == 비동기 문법
         */
        [HttpGet]
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
        [Route("{JobId}", Name ="JobByJobId")]
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

        // 추후엔 Spring에서 객체를 던져줘서 CreateJob(JobModel job) 으로 인자를 받아와야 할 것 같음.
        [HttpPost("create")]
        public async Task<ActionResult> CreateJob()
        {
            try
            {
                // 임시 테스트 코드
                JobModel job = new JobModel();

                byte[] bytes = { 0, 0, 0, 0, 1 };
                job.JobId = 100;
                job.JobName = "HelloWorld";
                job.IsUse = 0;
                job.WorkflowName = "HelloWorld.java";
                job.WorkflowBlob = bytes;
                job.Note = "HelloWorld 출력하기";
                job.SaveDate = DateTime.Now;
                job.UserId = 4;

                await _jobRepo.CreateJob(job);

                return Ok("Job 등록 성공!");
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateJob(int JobId)
        {
            try
            {
                var dbJob = await _jobRepo.GetJob(JobId);
                if (dbJob == null)
                    return NotFound();

                await _jobRepo.UpdateJob(JobId, dbJob);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteJob(int JobId)
        {
            try
            {
                var dbJob = await _jobRepo.GetJob(JobId);
                if (dbJob == null)
                    return NotFound();

                await _jobRepo.DeleteJob(JobId);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
