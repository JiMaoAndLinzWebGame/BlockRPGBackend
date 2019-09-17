using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace BlockRPGBackend.WSAPI
{
    /// <summary>
    /// WS会话
    /// </summary>
    public class Session : IDisposable
    {
        private string _RemoteIP;
        private WebSocket _WS;
        private List<Session> _AllSession;
        private MyDbContext _DbContext;
        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private Random _Ran;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteip"></param>
        /// <param name="ws"></param>
        /// <param name="allsessions"></param>
        /// <param name="dbcontext"></param>
        /// <param name="ran"></param>
        public Session(string remoteip, WebSocket ws, List<Session> allsessions, MyDbContext dbcontext, Random ran)
        {
            _RemoteIP = remoteip;
            _WS = ws;
            _Ran = ran;
            _AllSession = allsessions;
            _DbContext = dbcontext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteip"></param>
        /// <param name="allsessions"></param>
        /// <param name="ran"></param>
        public Session(string remoteip, List<Session> allsessions, Random ran)
        {
            _RemoteIP = remoteip;
            _Ran = ran;
            _AllSession = allsessions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <returns></returns>
        public async Task Connect(MyDbContext dbcontext)
        {
            _DbContext = dbcontext;
            await Run();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            _WS.Abort();
            _WS.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                var buff = new ArraySegment<byte>(new byte[1024 * 1024 * 1]);//1M缓冲区
                var result = await _WS.ReceiveAsync(buff, CancellationTokenSource.Token);
                if (result.MessageType != WebSocketMessageType.Text) continue;
                var msg = Encoding.UTF8.GetString(buff.Array).Trim();
                var msgbase = JsonConvert.DeserializeObject<Messages.MessageBase>(msg);
                switch (msgbase.MessageType)
                {
                    #region 查询方块
                    case Enums.MessageType.queryBlocks:
                        var param = JsonConvert.DeserializeObject<Messages.QueryBlocks>(msgbase.Params);
                        var blocks = _DbContext.Blocks.Where(obj => obj.X >= param.X && obj.X < (param.X + param.W) && obj.Y >= param.Y && obj.Y < (param.Y + param.H) && obj.MapId == param.MapID).ToList();
                        #region 循环查找空缺的blocks并随机生成填充
                        for (int x = 0; x < param.W; x++)
                        {
                            for (int y = 0; y < param.H; y++)
                            {
                                if (blocks.FirstOrDefault(obj => obj.X == x && obj.Y == y) != null) continue;
                                var newblock = new Modules.Blocks();
                                newblock.MapId = param.MapID;
                                newblock.X = x;
                                newblock.Y = y;
                                var cells = new Modules.Cell[22][];

                                for (int cellsx = 0; cellsx < cells.Length; cellsx++)
                                {
                                    cells[cellsx] = new Modules.Cell[14];
                                    for (int cellsy = 0; cellsy < cells[cellsx].Length; cellsy++)
                                    {
                                        cells[cellsx][cellsy] = new Modules.Cell();
                                        cells[cellsx][cellsy].X = cellsx;
                                        cells[cellsx][cellsy].Y = cellsy;
                                        cells[cellsx][cellsy].ResID = _Ran.Next(0, 2);//随机生成方块类型
                                    }
                                }
                                newblock.Cells = cells;
                                _DbContext.Blocks.Add(newblock);//.State==Microsoft.EntityFrameworkCore.EntityState.Added;
                                blocks.Add(newblock);
                            }
                        }
                        _DbContext.SaveChanges();

                        #endregion 
                        var messageresult = new Messages.MessageBase();
                        messageresult.MessageType = Enums.MessageType.queryBlocks;
                        messageresult.Params = JsonConvert.SerializeObject(blocks);
                        await _WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageresult))), WebSocketMessageType.Text, true, CancellationTokenSource.Token);
                        break;
                    #endregion
                    default://默认
                        break;
                }
            }//End While
        }
    }//End class
}