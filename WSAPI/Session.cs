using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

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
        private IRedisDatabase _Redis;

        private TaskQueue _MapAccessQueue = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteip"></param>
        /// <param name="ws"></param>
        /// <param name="allsessions"></param>
        /// <param name="dbcontext"></param>
        /// <param name="ran"></param>
        /// <param name="redis"></param>
        /// <param name="mapaccessqueue"></param>
        public Session(string remoteip, WebSocket ws, List<Session> allsessions, MyDbContext dbcontext, Random ran, IRedisDatabase redis, TaskQueue mapaccessqueue)
        {
            _RemoteIP = remoteip;
            _WS = ws;
            _Ran = ran;
            _AllSession = allsessions;
            _DbContext = dbcontext;
            _Redis = redis;
            _MapAccessQueue = mapaccessqueue;
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
        /// <param name="param">{ MessageType:1, Params:"{  X:0,  Y:0,  W:3,  H:3,  MapID:0 }"}</param>
        /// <returns></returns>
        private async Task<Modules.Blocks[]> queryBlocks(Messages.QueryBlocks param)
        {
            var blocks = new List<Modules.Blocks>(); // = _DbContext.Blocks.Where(obj => obj.X >= param.X && obj.X < (param.X + param.W) && obj.Y >= param.Y && obj.Y < (param.Y + param.H) && obj.MapId == param.MapID).ToList();

            #region 查询地图
            var rediswaithandles = new List<Task<Modules.Blocks>>();
            #region 首先在redis里查询block
            //循环添加redis查询任务
            {
                for (int x = 0; x < param.W; x++)
                {
                    for (int y = 0; y < param.H; y++)
                    {

                        rediswaithandles.Add(_Redis.HashGetAsync<Modules.Blocks>("Blocks", $"{param.MapID}:{param.X + x}:{param.Y + y}"));
                    }

                }
            }
            //一次性等待所有redis查询任务完成
            Task.WaitAll(rediswaithandles.ToArray());
            #endregion

            for (int x = 0; x < param.W; x++)
            {
                for (int y = 0; y < param.H; y++)
                {
                    #region 首先在redis返回的结果里查询block
                    var redisresult = (from waithandle in rediswaithandles where waithandle.Result.X == param.X + x && waithandle.Result.Y == param.Y + y select waithandle.Result).FirstOrDefault();
                    if (redisresult != null)
                    {
                        blocks.Add(redisresult);
                        continue;
                    }
                    #endregion

                    #region 如果redis找不到,就在mysql里找
                    {
                        var block = _DbContext.Blocks.FirstOrDefault(obj => obj.X == (param.X + x) && obj.Y == (param.Y + y) && obj.MapId == param.MapID);
                        if (block != null)
                        {
                            blocks.Add(block);
                            //如果找到了,就缓存到redis里
                            await _Redis.HashSetAsync<Modules.Blocks>("Blocks", $"{param.MapID}:{param.X + x}:{param.Y + y}", block);
                            continue;
                        }
                    }
                    #endregion

                    #region 如果redis和mysql都没有,则随机生成
                    {
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

                        await _DbContext.Blocks.AddAsync(newblock);//.State==Microsoft.EntityFrameworkCore.EntityState.Added;
                        await _Redis.HashSetAsync<Modules.Blocks>("Blocks", $"{param.MapID}:{param.X + x}:{param.Y + y}", newblock);
                        blocks.Add(newblock);
                    }
                    #endregion
                }//End For Y
            }//End For X
            _DbContext.SaveChanges();
            return blocks.ToArray();
            #endregion
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
                        var blocks = await _MapAccessQueue.AddTaskAsync2(queryBlocks, param);
                        var messageresult = new Messages.MessageBase()
                        {
                            MessageType = Enums.MessageType.queryBlocks,
                            Params = JsonConvert.SerializeObject(blocks)
                        };
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