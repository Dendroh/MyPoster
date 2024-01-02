using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Network;
using UnityEngine;

public class NetClient
{
    private const string UserName = "UnityClient";

    private LocalClient _client;

    public event ReceiveMessage OnReceiveMessage;

    private int RetryMaxCount = 5;
    private int RetryCount = 0;
    public bool status = false;
    public bool isRetry = false;

    public NetClient()
    {

    }

    public void Start()
    {
        _client = new LocalClient();
        _client.OnConnected += Connected;
        _client.OnReceiveObject += ReceiveObject;
        _client.OnDisconnected += Disconnected;
        _client.Start();
    }

    public void Close()
    {
        _client.Close();
    }

    private void Connected()
    {
        Debug.Log("연결됨\n");
        Connect connect = new Connect()
        {
            Type = PacketType.Connect,
            UserName = UserName,
        };

        _client.UserName = UserName;
        _client.SendPacket(connect);

        RetryCount = 0;
        status = true;
    }

    private void Disconnected(Exception ex)
    {
        Debug.Log("연결 끊어짐\n" + ex.ToString());

        status = false;

        Task.Run(async () => await RetryConnect());
    }

    private async Task RetryConnect()
    {
        while (RetryCount < RetryMaxCount) {
            if (status == true)   // 연결 완료된 상태면 return
                return;

            Debug.Log("연결 재시도\n");

            isRetry = true;

            if (_client != null) {
                _client = null;
            }

            Start();    // _client 초기화 및 연결

            RetryCount++;

            await Task.Delay(1000); // 1초 딜레이 타임
        }

        // 완전 연결이 안된 경우
        isRetry = false;
    }

    private void ConnectError(Exception ex)
    {
        Debug.Log("접속 에러\n" + ex.ToString());
    }

    private void ReceiveObject(Packet packet)
    {
        if (packet == null)
            return;

        switch (packet.Type)
        {
            case PacketType.Message: ReceiveMessage(packet); break;
        }
    }

    private void ReceiveMessage(Packet packet)
    {
        Message message = packet as Message;
        if (message == null)
            return;

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
        _client.SendPacket(packet);
    }
}

public delegate void ReceiveMessage(string message);
