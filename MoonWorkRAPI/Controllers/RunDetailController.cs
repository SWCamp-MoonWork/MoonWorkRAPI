using Microsoft.AspNetCore.Mvc;
using MoonWorkRAPI.Models;
using MoonWorkRAPI.Repository;

namespace MoonWorkRAPI.Controllers
{
    [ApiController]
    [Route("v1/RunDetail")]
    public class RunDetailController : ControllerBase
    {
        private readonly IRunDetailRepository _rundetailRepo;

        public RunDetailController(IRunDetailRepository rundetailRepo)
        {
            _rundetailRepo = rundetailRepo;
        }

        [HttpPost()]
        public object CreateRunDetail(RunDetailModel run)
        {
            try
            {
                _rundetailRepo.CreateDetail(run);
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
