using System;
using Google.Protobuf;
using Proto;
using Summer.Core;
using System.Net.Sockets;

namespace Summer.Network
{
    /// <summary>
    /// 客户端网络连接对象
    /// 职责：发送消息，接受消息回调，关闭连接，断开回调
    /// </summary>
    public class Connection : TypeAttributeStore
    {
        public delegate void DataReceivedCallback(Connection sender, IMessage msg);
        public delegate void DisconnectedCallback(Connection sender);

        // 接收到数据
        public DataReceivedCallback OnDataReceived;
        // 连接断开
        public DisconnectedCallback OnDisconnected;

        private Socket _socket;
        public Socket Socket
        {
            get
            {
                return _socket;
            }
        }


        public Connection(Socket socket)
        {
            this._socket = socket;

            // 创建解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += _received;
            lfd.Disconnected += OnDisconnectedHandler;
            lfd.Start();
        }

        public void OnDisconnectedHandler(Socket socket)
        {
            if (OnDisconnected != null)
                OnDisconnected(this);
        }


        private void _received(byte[] data)
        {
            //获取消息序列号
            ushort code = GetUShort(data, 0);
            var msg = ProtoHelper.ParseFrom(code, data, 2, data.Length - 2);

            if (MessageRouter.Instance.Running)
            {
                MessageRouter.Instance.AddMessage(this, msg);
            }

            OnDataReceived?.Invoke(this, msg);
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            _socket.Close();
            _socket = null;
            OnDisconnected?.Invoke(this);
        }

        #region 发送网络数据包相关代码
        private Package _package = null;

        public void Send()
        {
            if (_package != null)
                Send(_package);
            _package = null;
        }

        public void Send(IMessage message)
        {
            using (var ds = DataStream.Allocate())
            {
                int code = ProtoHelper.SeqCode(message.GetType());
                ds.WriteInt(message.CalculateSize() + 2);
                ds.WriteUShort((ushort)code);
                message.WriteTo(ds);
                this.SocketSend(ds.ToArray());
            }

        }

        //通过socket发送原生数据
        private void SocketSend(byte[] data)
        {
            this.SocketSend(data, 0, data.Length);
        }
        //通过socket发送原生数据
        private void SocketSend(byte[] data, int offset, int len)
        {
            lock (this)
            {
                if (_socket.Connected)
                {
                    _socket.BeginSend(data, offset, len, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {

        }
        #endregion

        //前提是data必须是大端字节序
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            }
        }
    }
}
