using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;
using System.Linq.Expressions;

namespace MoonWorkRAPI.Controllers
{
    [ApiController]
    [Route("v1/host")]
    public class HostController : ControllerBase
    {
        private readonly IHostRepository _HostRepo;

        public HostController(IHostRepository hostRepo)
        {
            _HostRepo = hostRepo;
        }

        //host 전체 불러오기
        [HttpGet("list")]
        public async Task<ActionResult<List<HostModel>>> GetHosts()
        {
            try
            {
                var host = await _HostRepo.GetHosts();
                if(host == null)
                {
                    return NotFound();
                }
                return Ok(host);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        //run 시킬 host 찾기
        [HttpGet]
        [Route("findtorun")]
        public async Task<ActionResult<List<HostModel>>> GetHost()
        {
            try
            {
                var host = await _HostRepo.GetHost();

                return Ok(host);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // hostid 찾기
        [HttpGet("findhostid")]
        public ActionResult GetHostId(long HostId)
        {
            try
            {
                var hostid = _HostRepo.GetHostId(HostId);
                if (hostid == null)
                    return NotFound();

                return Ok(hostid);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // IsUse가 true인 Host 추출
        [HttpGet("isusetrue")]
        public async Task<ActionResult<List<HostModel>>> GetHost_IsUseTrue()
        {
            try
            {
                var host = await _HostRepo.GetHost_IsUseTrue();
                return Ok(host);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }

        // HostId에 따른 Job의 정보 추출
        [HttpGet("getjob_hostid")]
        public async Task<ActionResult> GetHost_IsUseFalse(long HostId)
        {
            try
            {
                var host = await _HostRepo.GetJob_HostId(HostId);
                return Ok(host);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //특정 host에게 run 시킨 후 업데이트
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult> UpdateHost(HostModel host)
        {
            try
            {
                var dbhost = await _HostRepo.GetHost();
                if (dbhost == null)
                    return NotFound();
                await _HostRepo.UpdateHost(host);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}