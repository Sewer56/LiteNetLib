using System.Collections.Generic;

namespace LiteNetLib
{
    internal sealed class ReliableSequencedChannel
    {
        private int _localSequence;
        private int _remoteSequence;
        private readonly Queue<NetPacket> _outgoingPackets;
        private readonly NetPeer _peer;
        private bool _mustSendAck;

        public ReliableSequencedChannel(NetPeer peer)
        {
            _outgoingPackets = new Queue<NetPacket>();
            _peer = peer;
        }

        public void AddToQueue(NetPacket packet)
        {
            lock (_outgoingPackets)
            {
                _outgoingPackets.Enqueue(packet);
            }
        }

        public void SendNextPackets()
        {
            lock (_outgoingPackets)
            {
                while (_outgoingPackets.Count > 1)
                {
                    var packet = _outgoingPackets.Dequeue();
                    _localSequence = (_localSequence + 1) % NetConstants.MaxSequence;
                    packet.Sequence = (ushort)_localSequence;
                    _peer.SendRawData(packet);
                    _peer.Recycle(packet);
                }
            }

            if (_mustSendAck)
            {
                //
            }
        }

        public void ProcessPacket(NetPacket packet)
        {
            _mustSendAck = true;
        }
    }
}
