using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / Lock 기초
    class Thread06
    {
        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                // 상호배제 Mutual Exclusive
                // Critical Section (c++, std::mutex) 
                Monitor.Enter(_obj); //문을 잠그는 행위

                number++;

                //코드가 길어져서 실수로 중간에 이런식으로 return을 해버리면
                //Thread_2는 무한 대기 상태에 빠지고 이런 상태를 데드락 DeadLock이라고 함
                //return;

                // 아래처럼 예외상황에 의해 종료될 경우도 마찬가지로 위험하다.
                // return과 예외 모두 try-finally 방식으로 해결은 가능하다.
                // 하지만 번거롭기 때문에 Monitor는 거의 안쓴다고 함.. 대신 lock이 있음
                // number / 0; 

                Monitor.Exit(_obj); // 잠금을 풀어준다.
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                Monitor.Enter(_obj); 

                number--;

                Monitor.Exit(_obj);
            }
        }

        static void Thread_1_lock()
        {
            for (int i = 0; i < 100000; i++)
            {
                // Monitor와 효과는 같지만 lock은 알아서 들어갈때 잠궈주고 나갈때 풀어줌
                lock (_obj)
                {
                    number++;
                }
            }
        }

        static void Thread_2_lock()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (_obj)
                {
                    number--;
                }
            }
        }

        static void Main_temp(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}
