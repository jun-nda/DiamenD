
using Summer.Network;
using System.Net;
using System.Net.Sockets;
using Proto;

namespace Summer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetService netService = new NetService();
            netService.Start();

            MessageRouter.Instance.Start(4);

            // 消息订阅
            MessageRouter.Instance.On<UserLoginRequest>(OnUerLoginRequest);

            Console.ReadKey();
        }

        // 当消息分发器发现了.... 回调
        private static void OnUerLoginRequest(Connection sender, UserLoginRequest msg)
        {
            Console.WriteLine("发现用户登录请求: {0}, {1}", msg.Username, msg.Password);
        }
    }
}