using System;

namespace Unity.Network
{
    public enum PacketType
    {
        None,
        Connect, Reconnect, Disconnect,
        Message,
    }

    [Serializable]
    public class Packet
    {
        public PacketType Type;

        public Packet()
        {

        }

        public Packet(PacketType type)
        {
            Type = type;
        }
    }

    [Serializable]
    public class Connect : Packet
    {
        public string UserName;

        public Connect() : base(PacketType.Connect)
        {

        }
    }

    [Serializable]
    public class Message : Packet
    {
        public string Content;

        public Message() : base(PacketType.Message)
        {

        }
    }
}
