
using Summer.Network;
using System.Net;
using System.Net.Sockets;
using GameServer.Network;
using GameServer.Service;
using Proto;
using Serilog;

namespace Summer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //初始化日志环境 
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            Log.Information("start");
            
            //网路服务模块
            NetService netService = new NetService();
            netService.Start();
            Log.Debug("网络服务启动完成");

            UserService userService = UserService.Instance;
            userService.Start();
            Log.Debug("玩家服务启动完成");

            RoomService roomService = RoomService.Instance;
            roomService.Start();
            Log.Debug("房间服务启动完成");

            LockStepService lockStepService = LockStepService.Instance;
            lockStepService.Start();
            
            Console.ReadKey();
        }
    }
}