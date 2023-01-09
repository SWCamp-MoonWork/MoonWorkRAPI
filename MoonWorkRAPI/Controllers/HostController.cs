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

        //run 시킬 host 찾기
        [HttpGet]
        [Route("host/find")]
        public async Task<Action<List<HostModel>>> GetHost()
        {
            try
            {
                var host = await _HostRepo.GetHost();
                if (host == null)
                    return NotFound();

                return Ok(host);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //특정 host에게 run 시킨 후 업데이트
        [HttpPut]
        [Route("host")]
        public async Task<Action> UpdateSchedule()
        {
            try
            {
                var dbhost = await _HostRepo.GetHost();
                if (dbhost == null)
                    return NotFound();
                await _HostRepo.UpdateSchedule(dbhost);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
