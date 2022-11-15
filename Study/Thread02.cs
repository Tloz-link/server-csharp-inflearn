using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / 컴파일러 최적화
    class Thread02
    {
        //volatile을 붙이면 컴파일러가 _stop과 관련된 최적화를 하지 않음
        //C#에서의 volatile은 위의 동작 이외에도 더 복잡하게 작동하기 때문에 괜히 사용하다가 망가질 수 있다.
        //그러니 지금 이후로 일단 쓰지 말고 앞으로 배울 다른 방식으로 처리하자 
        volatile static bool _stop = false;  
        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");

            while (_stop == false)
            {
                // 누군가가 stop 신호를 해주기를 기다린다.
                // Release 모드에서는 여기를 빠져나가지 못함
                // 싱글일땐 상관없는데 멀티쓰레드에선 이런 문제가 많이 발생함
            }

            Console.WriteLine("쓰레드 종료!");
        }

        //_stop에 volatile을 붙이지 않을 경우 밑의 함수처럼 됨
        static void Release()
        {
            //_stop에 volatile을 붙이지 않을 경우
            //릴리즈 모드에서 컴파일러가 다음과 비슷하게 최적화를 해버림
            if (_stop == false)
            {
                while (true)
                {
                    // 이렇게 되면 다른 쓰레드가 stop 신호를 줘도 while에서 빠져나가지 못하게됨
                }
            }
        }

        static void Main_temp(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");
            t.Wait();

            Console.WriteLine("종료 성공");
        }
    }
}
