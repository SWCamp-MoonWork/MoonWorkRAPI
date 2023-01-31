using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using System.Linq.Expressions;

namespace MoonWorkRAPI.Controllers
{
    [ApiController]
    [Route("v1")]
    public class RunController : ControllerBase
    {
        private readonly IRunRepository _RunRepo;

        public RunController(IRunRepository RunRepo)
        {
            _RunRepo = RunRepo;
        }


        //run 시킨 후 run 정보 기록
        [HttpPost]
        [Route("run")]
        public ActionResult<RunModel> CreateRun(RunModel run)
        {
            try
            {
                _RunRepo.CreateRun(run);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //특정 RunId에 대한 정보 가져오기
        [HttpGet]
        [Route("run/{RunId}")]
        public async Task<ActionResult<RunModel>> GetRunInfo(int RunId)
        {
            try
            {
                var run = await _RunRepo.GetRunInfo(RunId);
                return Ok(run);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //특정 JobId로 인해 발생한 Run 조회
        [HttpGet]
        [Route("run/{JobId}/byjob")] //run/{JobId} 이렇게만 하면 경로가 같다고 인식해 오류 발생
        public async Task<ActionResult> GetRunbyJobId(int JobId)
        {
            try
            {
                var run = await _RunRepo.GetRunbyJobId(JobId);
                return Ok(run);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //일정 기간 동안의 모든 run 데이터 조회
        [HttpGet]
        [Route("run/history")]
        public async Task<ActionResult<List<RunModel>>> GetRunbyDate(DateTime FromDT, DateTime ToDT)
        {
            try
            {
                var run = await _RunRepo.GetRunbyDate(FromDT, ToDT);
                if (run == null)
                    return NotFound();

                return Ok(run);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //특정 RunId에 대한 정보 업데이트
        [HttpPut]
        [Route("run/{RunId}")]
        public ActionResult<RunModel> UpdateRun(RunModel run)
        {
            try
            {
                _RunRepo.UpdateRun(run);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}