using Microsoft.AspNetCore.Http;
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
        /// 
        /// </summary>
        /// <param name="remoteip"></param>
        /// <param name="ws"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task addSession(string remoteip, WebSocket ws, MyDbContext dbContext)
        {
            var session = new Session(remoteip, ws, _Sessions, dbContext, _Ran);
            _Sessions.Add(session);
            await session.Run();
            return;
        }//end addSession
    }//end class

}//end namespace