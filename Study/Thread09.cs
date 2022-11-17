using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / AutoResetEvent

    //AutoResetEvent - 톨게이트 (자동으로 열림)
    //ManualResetEvent - 문 (수동으로 열림)
    class Lock_Auto
    {
        // bool 이랑 비슷하다고 보면 된다.
        AutoResetEvent _available = new AutoResetEvent(true); // true = 톨게이트를 열어둔 채로 시작
        // 커널이 이벤트를 가지고 있다가 호출하는 작업자체가 부담이 있기 때문에 mmorpg같은 
        // 대규모 처리를 위한 서비스에는 어울리지 않는다.

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
            //_available.Reset(); // false로 바꿔주는 데 이게 WaitOne()함수에 세트로 들어있음
        }

        public void Release()
        {
            _available.Set(); // flag = true
        }
    }

    class Lock_Manual
    {
        ManualResetEvent _available = new ManualResetEvent(true); // true = 방문을 열어둔 채로 시작

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
            _available.Reset(); // 문을 자동으로 안 닫아주기 때문에 이걸 따로 실행해줘야 하지만
            // 위처럼 할경우 당연히 두 개 이상의 쓰레드가 문을 열고 들어갈 위험이 있다.
            // 그러므로 Manual 방식은 lock을 구현하는 것 보다는 다른 방식으로 사용하는 것이 좋다.
        }

        public void Release()
        {
            _available.Set(); // flag = true
        }
    }

    class Thread09
    {
        static int _num = 0;

        // AutoResetEvent와 달리 int로 한 쓰레드가 여러번 잠글수가 있다. 그러기 위해서 ThreadId까지 가지고 있다.
        static Mutex _lock = new Mutex(); // Mutex도 AutoResetEvent처럼 커널 동기화 객체기 때문에 성능 저하가 있음

        static void Thread_1()
        {
            for (int i = 0; i < 100000; ++i)
            {
                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; ++i)
            {
                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();
            }
        }

        static void Main_temp(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
