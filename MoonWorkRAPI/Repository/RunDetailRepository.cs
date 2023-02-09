using Dapper;
using MoonWorkRAPI.Context;
using MoonWorkRAPI.Models;

namespace MoonWorkRAPI.Repository
{

    public interface IRunDetailRepository
    {
        public void CreateDetail(RunDetailModel run);
    }
    public class RunDetailRepository : IRunDetailRepository
    {
        private readonly DapperContext _context;

        public RunDetailRepository(DapperContext context)
        {
            _context = context;
        }

        public void CreateDetail(RunDetailModel run)
        {
            var query = "INSERT INTO RunDetail(RunId, ExecuteDT, ResultData) " +
                "values(@RunId, @ExecuteDT, @ResultData)";

            using (var conn = _context.CreateConnection())
            {
                conn.Query<RunDetailModel>(query);
            }
        }
    }
}
