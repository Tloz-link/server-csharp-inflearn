using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession> ();
        object _lock = new object ();

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            // 이대로면 여러개의 클라이언트와 통신을 하고 싶어도 대부분의 쓰레드들이 여기서 막혀서 대기해야함
            // 멀티쓰레드의 장점을 살리지 못하는 셈 이 병목점을 해결하는 방법중 하나는 Job Queue를 활용하는 것!
            lock (_lock)
            {
                foreach (ClientSession s in _sessions)
                    s.Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
