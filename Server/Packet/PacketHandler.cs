using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        // 클라이언트가 꺼졌을 때 clientSession.Room가 null이 되어 문제가 되므로 미리 참조를 받아놓음
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.BroadCast(clientSession, chatPacket.chat)
        );
    }
}
