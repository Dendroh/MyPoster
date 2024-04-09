using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Unity.Network;

namespace Unity.Network
{
    public class NetServer
    {
        private readonly TcpListener listener;
        private LocalClient localClient;

        public event ReceiveMessage OnReceiveMessage;

        public NetServer()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, LocalClient.PortNumber);
        }

        public bool Start()
        {
            try
            {
                listener.Start();

                TcpClient tcpClient = listener.AcceptTcpClient();
                localClient = new LocalClient(tcpClient);
                localClient.OnReceiveObject += ReceiveObject;
                localClient.OnDisconnected += Disconnected;
                localClient.Start();
            }
            catch (Exception)
            {
                Debug.WriteLine("AAAAAAA");
                return false;
            }//try

            return true;
        }

        public void Close()
        {
            localClient.Close();
            listener.Stop();
        }

        private void Disconnected(Exception ex)
        {
            // 접속 종료되면 소켓닫고 다시 재 연결
            Close();
            Start();
        }

        private void ReceiveObject(Packet packet)
        {
            if (packet == null)
                return;

            switch (packet.Type)
            {
                case PacketType.Connect: Connected(packet); break;
                case PacketType.Message: ReceiveMessage(packet); break;
            }
        }

        private void Connected(Packet packet)
        {
            Connect connect = packet as Connect;
            if (connect == null)
                return;

            localClient.UserName = connect.UserName;
        }

        private void ReceiveMessage(Packet packet)
        {
            Message message = packet as Message;
            if (message == null)
                return;

            Debug.WriteLine(message.Content);

            if (OnReceiveMessage != null)
                OnReceiveMessage(message.Content);
        }

        public void SendMessage(string message)
        {
            Message packet = new Message()
            {
                Content = message,
            };
            SendPacket(packet);
        }

        private void SendPacket(Packet packet)
        {
            localClient.SendPacket(packet);
        }
    }

    public delegate void ReceiveMessage(string message);
}
