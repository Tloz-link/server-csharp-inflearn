using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //recvArgs.UserToken = this;  //이렇게 사용하면 나중에 어느 세션에서 온 args인지 알 수 있다.
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(recvArgs);
        }

        // 임시로 블로킹 버전으로 둔다.
        // Accept, Recv는 바로 Async로 실행해서 대기 상태에 있다가 네트워크 통신이 오면 그때그때 처리하면 되지만
        // Send는 사용자가 원할 때 실행해야 하므로 지금까지랑은 조금 다르다.
        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            int expected = 0;
            int desired = 1;
            if (Interlocked.CompareExchange(ref _disconnected, desired, expected) == expected)
            {
                _socket.Shutdown(SocketShutdown.Both); // recv, send 둘다 안하겠다고 미리 알려준다.
                _socket.Close();
                // 이 세션의 socket을 여러번 disconnect하는걸 방지하는 거니까 다시 _disconnected를 0으로 바꿔주면 안된다.
                // 그래야 여러 쓰레드에서 시도해도 단 한번만 실행되고 끝남
            }
        }

        #region 네트워크 통신

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvCompleted(null, args);
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                //TODO
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                // TODO Disconnect
            }
        }
        #endregion
    }
}
