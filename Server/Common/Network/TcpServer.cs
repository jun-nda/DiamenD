using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Summer.Network;

namespace Summer
{
    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// </summary>
    public class TcpServer
    {
        private IPEndPoint endPoint;
        private Socket serverSocket;    //服务端监听对象
        private int backlog = 100;


        public delegate void ConnectedCallback(Connection conn);
        public delegate void DataReceivedCallback(Connection conn, IMessage msg);
        public delegate void DisConnectedCallback(Connection conn);


        public event EventHandler<Socket> SocketConnected; //客户端接入事件
        public ConnectedCallback Connected;
        public DataReceivedCallback DataReceived;
        public DisConnectedCallback Disconnected;


        public TcpServer(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public TcpServer(string host, int port, int backlog):this(host,port)
        {
            this.backlog = backlog;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(backlog);
                Console.WriteLine("开始监听端口：" + endPoint.Port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAccept; //当有人连入的时候
                serverSocket.AcceptAsync(args);
                
            }
        }

        private void OnAccept(object? sender, SocketAsyncEventArgs e)
        {

            Socket client = e.AcceptSocket; //连入的人
            // 异步
            //继续接收下一位
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);

            //真的有人连进来
            if (e.SocketError == SocketError.Success)
            {
                if (client!=null)
                {
                    OnSocketConnected(client);
                }
                
            }
        }

        private void OnSocketConnected(Socket socket)
        {
            SocketConnected?.Invoke(this, socket);
            Connection conn = new Connection(socket);
            conn.OnDataReceived += (conn, data) => DataReceived?.Invoke(conn, data);
            conn.OnDisconnected += (conn) => Disconnected?.Invoke(conn);
            Connected?.Invoke(conn);

        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            if (serverSocket == null)
                return;
            serverSocket.Close();
            serverSocket = null;
        }

    }
}
