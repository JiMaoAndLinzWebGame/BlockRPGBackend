using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace BlockRPGBackend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext _DbContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbcontext"></param>
        public TestController(MyDbContext dbcontext)
        {
            _DbContext = dbcontext;
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
            var user = new Modules.Users()
            {
                UserName = "TestUsers",
                Password = "123",
                RegisterIP = "1",
                Token = "2",
                Amount = 0,
                RegisterTime = DateTime.Now,
                LastLoginTime = DateTime.Now,
                status = 0
            };
            _DbContext.Users.Add(user);
            _DbContext.SaveChanges();
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