using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using UnityEngine;
using Summer;
using Summer.Network;
using Proto;
using UnityEngine.Rendering;

public class NetClient : Singleton<NetClient> {
	static Connection conn = null;

	public Connection Conn {
		get {
			return conn;
		}
	}

	public static void Send (IMessage message) {
		if (conn != null) {
			conn.Send(message);
		}
	}
	
	public void ConnectToServer (string host, int port) {
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect(iPEndPoint);
		Debug.Log("连接到服务端");
		conn = new Connection(socket);
		conn.OnDisconnected += OnDisconnected;
	}
	
	void OnDisconnected (Connection sender) {
		Debug.Log("与服务器断开");
	}

	/// <summary>
	/// 关闭网络客户端
	/// </summary>
	public static void Close () {
		conn?.Close();
	}
}
