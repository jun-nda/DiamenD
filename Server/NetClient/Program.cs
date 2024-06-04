// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using Google.Protobuf;
using Proto;
using Summer.Network;
using Serilog;
using Summer;

//初始化日志环境 
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.Console()
.WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();
static void SendMessage(Socket socket, byte[] body)
{
    int len = body.Length;
    byte[] lenBytes = BitConverter.GetBytes(len);
    socket.Send(lenBytes);
    socket.Send(body);
}


var host = "127.0.0.1";
int port = 32510;
IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(iPEndPoint);

Log.Debug("成功连接到服务器");

// 先等待一会，等服务端启动好了，再发消息
Thread.Sleep(1000);
;
Connection conn = new Connection(socket);


var msg = new UserLoginRequest();
msg.Username = "bac";
msg.Password = "123";
conn.Send(msg);

/*var pack = ProtobufTool.Pack(msg);
var res = ProtobufTool.Unpack(pack);
Log.Information("{0}", res);*/

Console.ReadLine();
