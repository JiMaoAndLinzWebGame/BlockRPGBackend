using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlockRPGBackend.WSAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskQueue : IDisposable
    {
        private ConcurrentQueue<Action> _taskqueue = new ConcurrentQueue<Action>();
        private bool _allowaddtask = true;
        /// <summary>
        /// 
        /// </summary>
        public TaskQueue()
        {
            DoWork();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        public void AddTask<T>(Action<T> task, T args)
        {
            _taskqueue.Enqueue(new Action(() => task(args)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="argv"></param>
        public Task<TReturn> AddTaskAsync<T, TReturn>(Func<T, TReturn> task, T argv)
        {
            return Task.Factory.StartNew<TReturn>(() =>
              {
                  var ret = default(TReturn);
                  var s = new SemaphoreSlim(0, 1);
                  //监测任务返回
                  _taskqueue.Enqueue(new Action(() =>
                    {
                        ret = task(argv);//保存返回值
                        s.Release();
                    }));
                  //循环等待任务返回
                  s.Wait();
                  return ret;
              });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="argv"></param>
        public Task<TReturn> AddTaskAsync2<T, TReturn>(Func<T, Task<TReturn>> task, T argv)
        {
            return Task.Factory.StartNew<TReturn>(() =>
              {
                  var ret = default(TReturn);
                  var s = new SemaphoreSlim(0, 1);
                  Exception e = null;
                  //监测任务返回
                  _taskqueue.Enqueue(new Action(() =>
                    {
                        try
                        {
                            var t = task(argv);
                            t.Wait();
                            ret = t.Result;//保存返回值
                        }
                        catch (Exception err) { e = err; };
                        s.Release();
                    }));
                  //循环等待任务返回
                  s.Wait();
                  if (e != null) throw e;
                  return ret;
              });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Action GetWork()
        {
            while (true)
            {
                if (_taskqueue.TryDequeue(out Action task)) return task;
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Action GetWorkOrDefault()
        {
            if (_taskqueue.TryDequeue(out Action task)) return task;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task<Action> GetWorkAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_taskqueue.TryDequeue(out Action task)) return task;
                    Thread.Sleep(10);
                }
            });
        }

        private async void DoWork()
        {
            while (true)
            {
                try
                {
                    var work = await GetWorkAsync();
                    work();
                }
                catch (Exception err)
                {
                    err.ToString();
                }
            }
        }

    }//End Class
}