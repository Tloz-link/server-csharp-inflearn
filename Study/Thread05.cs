using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / Interlocked
    class Thread05
    {
        // 가시성만으로는 모든 문제를 해결할 수 없음
        // 여러 쓰레드가 하나의 변수를 동시에 Load해서 각자 사용하면 문제가 발생한다.
        // 이름 Race Condition(경합 조건)이라고 함

        static int number = 0;
        
        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
                number++;

            //Thread_1과 Thread_2가 동시에 number를 가져가서 각자 처리하고 저장하면
            //0이 아니라 +1 혹은 -1이 된다.
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
                number--;
        }


        static void Thread_1_solve()
        {
            // atomic = 원자성

            // 집행검 User2 인벤에 넣어라 - OK
            // 집행검 User1 인벤에서 없애라 - FAIL
            // 위 동작은 반드시 한번에 일어나야함 이런걸 원자성을 보장한다고 함

            for (int i = 0; i < 100000; i++)
            {
                // All or Nothing
                // 원자성만 보장해주는 것이 아니라 접근 순서도 보장해줌
                // ref를 사용하지 않으면 number를 복사하는 순간 언제 값이 바뀌어있을 지 모름
                // 그래서 ref를 사용하여 number를 참조해서 항상 올바른 값에서 1을 증가시킴을 보장해줌
                Interlocked.Increment(ref number);
            }
        }

        static void Thread_2_solve()
        {
            for (int i = 0; i < 100000; i++)
            {
                //afterValue에 들어가는 값은 항상 이 함수에서 number를 1더한 값임을 보장한다.
                int afterValue = Interlocked.Decrement(ref number);

                //int next = number;  //이렇게 따로 사용하면 number는 쓰레드 공용 변수이기 때문에 어떤 값이 들어올지 보장할 수 없다.
            }
        }

        static void Main_temp(string[] args)
        {
            //number++;             //이 코드는

            //int temp = number;    //이런식으로 나뉘어서 일어난다.
            //temp += 1;
            //number = temp;


            //Problem_Main();
            //Solve_Main();
        }

        static void Problem_Main()
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }

        static void Solve_Main()
        {
            Task t1 = new Task(Thread_1_solve);
            Task t2 = new Task(Thread_2_solve);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }


    }
}
