using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            // 문지기
            // AddressFamily = ipv4, ipv6을 고르는 것 우리는 Dns가 준 패밀리를 그대로 사용함
            // SocketType, ProtocalType을 각각 Stream, Tcp로 두면 tcp 정책으로 소켓을 생성함
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수 (대기열)
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; ++i)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        // 서버나 클라이언트가 Accpet같은 블로킹 함수를 사용하면
        // 여러 유저를 처리하는데 있어서 에로사항이 많기때문에
        // 비동기 함수로 처리해 주어야 한다. -> 그만큼 고려해야 할게 많아짐
        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // 이벤트 재사용 시 불순물 제거

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false) // false라는 건 accept를 실행하자마자 운 좋게 pending(지연)없이 바로 완료가 되었다는 뜻이다. 
                OnAcceptCompleted(null, args); // 그래서 직접 호출해주면 됨
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            // 하나 처리 끝났으니 다음 클라 대기
            RegisterAccept(args);
        }
    }
}
