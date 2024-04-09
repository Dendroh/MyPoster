using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Unity.Network
{
    public class LocalClient
    {
        private const string LocalIp = "127.0.0.1";
        public const int PortNumber = 10001;
        private const int BufferSize = 2048;

        private NetworkStream _stream;
        public NetworkStream Stream
        {
            get { return _stream; }
        }

        private readonly BinaryFormatter _formatter;

        private TcpClient _client;
        private byte[] _buffer;

        public string UserName { get; set; }

        public event Connected OnConnected;
        public event Disconnected OnDisconnected;
        public event ConnectError OnConnectError;
        public event ReceiveObject OnReceiveObject;

        /// <summary>
        /// 클라이언트용 생성자
        /// </summary>
        public LocalClient() : this(null)
        {

        }

        /// <summary>
        /// 서버용 생성자
        /// </summary>
        /// <param name="client">이미 생성된 TcpClient</param>
        public LocalClient(TcpClient client)
        {
            _client = client;

            _formatter = new BinaryFormatter();
            _formatter.Binder = new PacketBinder();

            _buffer = new byte[BufferSize];
        }

        public void Start()
        {
            if (_client != null)
            {
                _stream = _client.GetStream();

                //서버 -> 클라이언트
                BeginRead();
            }
            else
            {

                //클라이언트 -> 서버
                StartClient();
            }
        }

        private void StartClient()
        {
            //클라이언트 -> 서버
            _client = new TcpClient();

            try
            {
                _client.BeginConnect(LocalIp, PortNumber, EndConnect, null);
            }
            catch (SocketException ex)
            {
                if (OnConnectError != null)
                    OnConnectError(ex);
            }
        }

        private void EndConnect(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);
                _stream = _client.GetStream();

                if (OnConnected != null)
                    OnConnected();


                BeginRead();
            }
            catch (SocketException ex)
            {
                if (OnConnectError != null)
                    OnConnectError(ex);
            }
        }

        private void BeginRead()
        {
            _stream.BeginRead(_buffer, 0, BufferSize, new AsyncCallback(ReadObject), null);
        }

        public void Close()
        {
            _stream.Close();
            _client.Close();
        }

        private void ReadObject(IAsyncResult result)
        {
            int readSize = 0;

            try
            {
                lock (_stream)
                {
                    readSize = _stream.EndRead(result);
                }

                if (readSize < 1)
                    throw new Exception("Disconnect");


                Packet packet = null;
                using (MemoryStream stream = new MemoryStream(_buffer))
                {
                    packet = (Packet)_formatter.Deserialize(stream);
                }

                if (OnReceiveObject != null)
                    OnReceiveObject(packet);


                lock (_stream)
                {
                    BeginRead();
                }
            }
            catch (Exception ex)
            {
                if (OnDisconnected != null)
                    OnDisconnected(ex);
            }
        }

        public void SendPacket(Packet packet)
        {
            byte[] data = null;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    _formatter.Serialize(stream, packet);

                    data = stream.ToArray();
                }

                if (data == null || data.Length < 1)
                    return;

                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }
            catch
            {

            }//try
        }
    }

    public delegate void Connected();
    public delegate void Disconnected(Exception ex);
    public delegate void ConnectError(SocketException ex);
    public delegate void ReceiveObject(Packet packet);
}
