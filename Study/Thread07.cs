using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / DeadLock

    // 이런 데드락은 예방하는 것은 쉽지 않고 데드락이 발생했을 때 분석해서 해결한다.
    // 이런식으로 무조건 데드락이 일어나게 짜놓고 release하는 경우는 거의 없고
    // 대부분 Debug때는 안잡히다가 release때 잡히게 된다.
    // 라이브로 나가기 전 개발 과정에서 데드락이 발생할 수 있는 상황을 발견하는 것이 중요
    class SessionManager
    {
        static object _lock = new object();

        static public void TestSession()
        {
            lock (_lock)
            {

            }
        }

        static public void Test()
        {
            lock (_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock = new object();

        static public void Test()
        {
            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }

        static public void TestUser()
        {
            lock (_lock)
            {

            }
        }
    }

    class Thread07
    {
        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                UserManager.Test();
            }
        }

        static void Main_temp(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            // 둘이 같이 실행될때만 데드락 발생
            t1.Start();
            t2.Start();

            // 이런식으로 쓰레드를 따로 쓸 경우에는 데드락이 발생하지 않아서 괜찮다고 생각하고 넘어가게 됨
            //t1.Start();
            //Thread.Sleep(100);
            //t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine("good");
        }
    }
}
