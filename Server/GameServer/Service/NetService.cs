
using System.Net.Sockets;
using GameServer.Model;
using Google.Protobuf;
using Proto;
using Summer.Network;

namespace Summer
{
    /// <summary>
    /// 网络服务
    /// 职责：启动端口监听，启动路由，接收Socket连入
    /// </summary>
    internal class NetService
    {
        TcpServer tcpServer = null;

        public NetService()
        {
            tcpServer = new TcpServer("0.0.0.0", 32510);
            tcpServer.Connected += OnClientConnected;
            tcpServer.Disconnected += OnDisconnected;
            tcpServer.DataReceived += OnDataReceived;
        }
        public void Start()
        {
            tcpServer.Start();
            MessageRouter.Instance.Start(10);

        }

        /// <summary>
        /// 当客户端接入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socket"></param>
        private void OnClientConnected(Connection conn)
        {
            Console.WriteLine("客户端接入:" + conn.Socket.RemoteEndPoint);
        }

        private void OnDataReceived(Connection conn, IMessage data)
        {

        }

        private void OnDisconnected(Connection conn)
        {
            Console.WriteLine("连接断开");
            var space = conn.Get<Room>();
            if (space != null)
            {
                var co = conn.Get<Character>();
                space.CharacterLeave(conn, co);
            }
        }
    }
}
