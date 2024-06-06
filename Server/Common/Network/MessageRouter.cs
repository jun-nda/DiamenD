using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Proto;
using System.Reflection;
using System.Threading;
using Serilog;

namespace Summer.Network
{
    class Msg
    {
        public Connection sender;
        public IMessage message;
    }

    /// <summary>
    /// 消息路由
    /// 职责：负责订阅与消息转发
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        private int _threadCount; // 工作线程数
        private int _workerCount; // 正在工作的线程数
        private bool _isRunning = false; // 是否正在运行
        private AutoResetEvent threadEvent = new AutoResetEvent(true); // 通过Set来唤醒,每次只能唤醒一个

        public bool Running
        {
            get
            {
                return _isRunning;
            }
        }

        /// 消息处理器
        public delegate void MessageHandler<T>(Connection sender, T msg);
        // 消息队列，所有客户端发来的消息都暂存在这里
        private Queue<Msg> messageQueue = new Queue<Msg>();
        // 频道字典 (订阅记录)
        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();

        // 订阅
        public void Subscribe<T>(MessageHandler<T> handler) where T : IMessage
        {
            string type = typeof(T).FullName;
            if(!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            delegateMap[type] = (delegateMap[type] as MessageHandler<T>) + handler;
            Console.WriteLine("消息订阅:" + type + ":" + delegateMap[type].GetInvocationList().Length);

        }

        // 触发
        void Fire<T>(Connection sender, T msg)
        {
            Log.Information("string");
            string type = typeof (T).FullName;
            if (delegateMap.ContainsKey(type))
            {
                MessageHandler<T> handler = (MessageHandler<T>)delegateMap[type];
                try
                {
                    handler?.Invoke(sender, msg);
                }catch (Exception ex)
                {
                    Console.WriteLine("MessageRouter.Fire error: " + ex.ToString() + ex.StackTrace);
                }
            }
        }

        // 退订
        public void Off<T>(MessageHandler<T> handler) where T : IMessage
        {
            string type = typeof(T).FullName;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            delegateMap[type] = (delegateMap[type] as MessageHandler<T>) - handler;
        }

        /// <summary>
        /// 添加新的消息到队列中
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">消息对象</param>
        public void AddMessage(Connection sender, IMessage message)
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue(new Msg() { sender = sender, message = message });
            }
            threadEvent.Set(); // 唤醒一个worker
        }

        public void Start(int threadCount)
        {
            if (_isRunning) return;
            _isRunning = true;
            _threadCount = Math.Min(Math.Max(threadCount,1), 200);
            for(int i = 0; i < _threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }

            while (_workerCount < this._threadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            messageQueue.Clear();
            while (_workerCount > 0)
            {
               threadEvent.Set();
               Thread.Sleep(10);
            }
        }

        private void MessageWork(object? state)
        {
            Console.WriteLine("MessageWork thread start");
            try
            {
                Interlocked.Increment(ref _workerCount);
                while (_isRunning)
                {
                    if(messageQueue.Count == 0) {
                        threadEvent.WaitOne(); // 线程等待,不能百分百相信,所以continue重新来一次
                        continue;
                    }
                    // 从消息队列中取出一个元素
                    Msg msg = null;
                    lock(messageQueue)
                    {
                        if (messageQueue.Count == 0) continue;
                        Log.Information("executeMessage"+ Thread.CurrentThread.ManagedThreadId.ToString() + messageQueue.Count);

                        msg = messageQueue.Dequeue();
                    }

                    IMessage package = msg.message;
                    if (package != null)
                    {
                        executeMessage(msg.sender, package);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Interlocked.Decrement(ref _workerCount);
            }


            Console.WriteLine("worker thread end");
        }

        // 递归处理消息
        private void executeMessage(Connection sender, IMessage message)
        {
            var fireMethod = this.GetType().GetMethod("Fire",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var met = fireMethod.MakeGenericMethod(message.GetType());
            Log.Information("invoke");
            met.Invoke(this, new object[] { sender, message });

            var t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                // 过滤属性
                if (p.Name == "Parser" || p.Name == "Descriptor") continue;

                object value = p.GetValue(message);
                if (value is IMessage value1)
                {
                    executeMessage(sender, value1);
                }
            }
        }
    }
}