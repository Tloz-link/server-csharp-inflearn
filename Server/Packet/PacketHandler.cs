using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        // 클라이언트가 꺼졌을 때 clientSession.Room가 null이 되어 문제가 되므로 미리 참조를 받아놓음
        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");
        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}
