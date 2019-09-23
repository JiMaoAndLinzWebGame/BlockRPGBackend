using BlockRPGBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace BlockRPGBackend.WSAPI
{
    /// <summary>
    /// 
    /// </summary>
    public static class Server
    {
        private static Random _Ran = new Random((int)DateTime.Now.Ticks);
        private static List<Session> _Sessions = new List<Session>();

        /// <summary>
        /// 地图访问队列
        /// </summary>
        /// <returns></returns>
        public static TaskQueue _MapAccessQueue = new TaskQueue();

        static Server()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteip"></param>
        /// <param name="ws"></param>
        /// <param name="dbcontext"></param>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        /// <returns></returns>
        public static async Task addSession(string remoteip, WebSocket ws, MyDbContext dbcontext, IConfiguration configuration, IRedisDatabase redis)
        {
            var session = new Session(remoteip, ws, _Sessions, dbcontext, _Ran, redis, _MapAccessQueue);
            _Sessions.Add(session);
            await session.Run();
            return;
        }//end addSession
    }//end class
}//end namespace