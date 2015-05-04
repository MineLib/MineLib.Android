using System;
using System.Net.Sockets;
using System.Security;
using MineLib.Network.IO;

namespace MineLib.Android
{
    public sealed class NetworkTCPAndroid : INetworkTCP
    {
        public bool Available { get { return _client != null && _client.Client.Available > 0; } }
        public bool Connected { get { return _client != null && _client.Client.Connected; } }

        private TcpClient _client;
        private NetworkStream _stream;

        public void Connect(string ip, ushort port)
        {
            _client = new TcpClient(ip, port);

            _stream = new NetworkStream(_client.Client);
        }
        public void Disconnect(bool reuse)
        {
            _client.Client.Disconnect(reuse);
        }


        public IAsyncResult BeginConnect(string ip, ushort port, AsyncCallback callback, object obj)
        {
            _client = new TcpClient();
            return _client.BeginConnect(ip, port, callback, obj);
        }
        public void EndConnect(IAsyncResult result)
        {
            _client.EndConnect(result);

            _stream = new NetworkStream(_client.Client);
        }

        public IAsyncResult BeginDisconnect(bool reuse, AsyncCallback callback, object obj)
        {
            _client = new TcpClient();
            return _client.Client.BeginDisconnect(reuse, callback, obj);
        }
        public void EndDisconnect(IAsyncResult result)
        {
            _client.Client.EndDisconnect(result);
        }


        public IAsyncResult BeginSend(byte[] bytes, int offset, int count, AsyncCallback callback, object obj)
        {
            return _stream.BeginWrite(bytes, offset, count, callback, obj);
        }
        public void EndSend(IAsyncResult result)
        {
            _stream.EndWrite(result);
        }

        public IAsyncResult BeginReceive(byte[] bytes, int offset, int count, AsyncCallback callback, object obj)
        {
            return _stream.BeginRead(bytes, offset, count, callback, obj);
        }
        public int EndReceive(IAsyncResult result)
        {
            return _stream.EndRead(result);
        }


        public void Send(byte[] bytes, int offset, int count)
        {
            _stream.Write(bytes, offset, count);
        }

        public int Receive(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Close();

            if (_stream != null)
                _stream.Dispose();
        }
    }
}