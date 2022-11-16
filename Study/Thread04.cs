using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / 메모리 배리어(하드웨어 최적화)
    class Thread04
    {
        // 메모리 배리어
        // A) 코드 재배치 억제
        // B) 가시성 (하나의 쓰레드가 변경한 메모리를 모든 쓰레드가 볼 수 있다)

        // 1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 둘다 막는다.
        // 2) Store Memory Barrier (ASM SFENCE) : Store만 막는다.
        // 3) Load Memory Barrier (ASM LFENCE) : Load만 막는다.

        // 풀 메모리 배리어가 가장 많이 쓰이고 2,3번은 많이 쓸일이 없을 것이다.

        //volatile을 변수 앞에 붙여도 해당 변수에 대한 가시성을 해결해줌 (코드 재배치 억제는 못함)

        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            // CPU 입장에서 밑의 코드를 보면 두 줄이 아무런 연관도 없기 때문에
            // CPU 자체의 성능 향상을 위해 r1 = x; 코드를 먼저 실행하도록 최적화 할 수 있다.
            y = 1; // Store y

            /////////// 위 아래 코드의 순서를 강제시키자 (메모리 배리어)

            r1 = x; // Load x
        }

        static void Thread_2()
        {
            // 여기도 마찬가지
            // 멀티쓰레드 이기 때문에 최적화가 문제가 됨 (r1과 r2가 동시에 0이 되어서 나와버림)
            x = 1; // Store x
            r2 = y; // Load y
        }

        static void Thread_1_solve()
        {
            y = 1; // Store y

            // 코드 순서를 강제함 (최적화 금지)
            Thread.MemoryBarrier();

            r1 = x; // Load x
        }

        static void Thread_2_solve()
        {
            x = 1; // Store x

            // x를 저장하고 나서 이를 메모리에 저장하고, y를 로드하기 전에 메모리를 확인함
            Thread.MemoryBarrier();

            r2 = y; // Load y
        }

        static void Main_temp(string[] args)
        {
            //Problem_Main();
            //Solve_Main();
        }

        static void Problem_Main()
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번 만에 빠져나옴!");
        }

        static void Solve_Main()
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1_solve);
                Task t2 = new Task(Thread_2_solve);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번 만에 빠져나옴!");
        }

        //메모리 배리어 유명 예제
        static int _answer;
        static bool _complete;

        static void A()
        {
            _answer = 123;
            Thread.MemoryBarrier(); // Barrier 1 (Store)
            _complete = true;
            Thread.MemoryBarrier(); // Barrier 2 (Store)
        }

        static void B()
        {
            Thread.MemoryBarrier(); // Barrier 3 (Load)
            if (_complete)
            {
                Thread.MemoryBarrier(); // Barrier 4 (Load)
                Console.WriteLine(_answer);
            }
        }
    }
}
