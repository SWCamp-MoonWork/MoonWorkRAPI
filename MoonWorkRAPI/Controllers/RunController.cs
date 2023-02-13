using java.util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using System.Linq.Expressions;

namespace MoonWorkRAPI.Controllers
{
    public class RunId
    {
        public long run{ set; get; } 
    }
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
        public object CreateRun(RunModel run)
        {
            try
            {
                var regist = _RunRepo.CreateRun(run);
/*                Console.WriteLine("run : " + run);
                Console.WriteLine("regist : " + regist);

                var obj = new RunId
                {
                    run = Convert.ToInt64(regist)
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
*/

                Console.WriteLine(regist);
                return Ok(regist);
            }
                catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //특정 RunId에 대한 정보 가져오기
        [HttpGet]
        [Route("run/{RunId}")]
        public async Task<ActionResult<RunModel>> GetRunInfo(long RunId)
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
        public async Task<ActionResult> GetRunbyJobId(long JobId)
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

        // job의 run 기록 최근 5개
        [HttpGet("run/{JobId}/getrunrecord")]
        public async Task<ActionResult> GetJob_RunRecord(long JobId)
        {
            try
            {
                var run = await _RunRepo.GetJob_RunRecord(JobId);
                return Ok(run);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // job의 run 기록 최근 20개 + duration
        [HttpGet("run/{JobId}/getduration")]
        public async Task<ActionResult> GetJob_Duration(long JobId)
        {
            try
            {
                var run = await _RunRepo.GetJob_Duration(JobId);
                return Ok(run);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // 최근 1시간동안 실행했던 작업들의 run 시작시간, 듀레이션
        [HttpGet("run/getgraph")]
        public async Task<ActionResult> GetGraph()
        {
            try
            {
                var run = await _RunRepo.GetGraph();
                return Ok(run);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


/*        // run 테이블에서 startdt enddt 듀레이션 구하기
        [HttpGet("run/{JobId}/getduration")]
        public async Task<ActionResult> GetDuration(long JobId)
        {
            try
            {
                var run = await _RunRepo.GetDuration(JobId);
                return Ok(run);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }*/

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

        /*        // job run이 다 돌면 endDT 업데이트
                [HttpPut("run/updateenddt")]
                public ActionResult<RunModel> Update_EndDT(RunModel run)
                {
                    try
                    {
                        _RunRepo.UpdateEndDT(run);

                        return NoContent();
                    }
                    catch(Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }*/

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