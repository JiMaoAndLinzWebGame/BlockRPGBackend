using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlockRPGBackend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext DbContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbcontext"></param>
        public TestController(MyDbContext dbcontext)
        {
            DbContext = dbcontext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="aa"></param>
        /// <returns></returns>
        [HttpGet]
        public string queryTest(string code, int aa)
        {
            DbContext.Users.Add(new Modules.Users() { UserName = "TestUsers" });
            DbContext.SaveChanges();
            return code + aa.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string queryTestA()
        {
            //DbContext.Order.Add(new Modules.Order(){Code=code});
            //DbContext.SaveChanges();
            return "456";
        }

    }
}