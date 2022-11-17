using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / Thread Local Storage

    // [ JobQueue ]
    // 잡큐에 lock을 걸고 일감을 빼올 때 한개가 아닌 여러개의 일감을 뽑아와서 각자 쓰레드에서 처리한다.

    class Thread12
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>();

        static void WhoAmI()
        {
            // Value 안에 값을 넣어서 사용가능
            // 이렇게 하면 쓰레드가 생성될 때마다 Value를 계속 덮어쓴다.
            ThreadName.Value = $"My name is {Thread.CurrentThread.ManagedThreadId}";

            //만약 ThreadLocal을 사용하지 않고 했다면 모든 쓰레드가 공유하는 변수가 되므로
            //Sleep이후에 consol.write를 하면 ThreadName가 하나로 통일될 것이다.
            //지금은 ThreadLocal을 사용했기 때문에 각기 다른 값을 출력할 것이다.
            Thread.Sleep(1000);

            Console.WriteLine(ThreadName.Value);
        }



        // 생성자로 func를 넣으면 ThreadName의 Value를 사용하려고 했을 때 null이 들어있다면 return값을 대입해준다.
        // 이미 해당 ThreadLocal 변수를 사용한 적이 있는 쓰레드가 다시 실행되었을 시 기존에 넣어져 있던 값을 그대로 사용한다.
        static ThreadLocal<string> ThreadName1 = new ThreadLocal<string>(() => { return $"My name is {Thread.CurrentThread.ManagedThreadId}"; });

        static void WhoAmI1()
        {
            bool repeat = ThreadName1.IsValueCreated; // Value가 null이면 false
            if (repeat)
                Console.WriteLine(ThreadName1.Value + " (repeat)");
            else
                Console.WriteLine(ThreadName1.Value);
        }



        static void Main(string[] args)
        {
            //이렇게 쓰레드풀 수를 제한하면 겹치는 이름이 나옴
            //똑같은 쓰레드가 여러가지 Task를 처리했기 때문
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);

            // 여러개의 Action들을 Task로 만들어서 실행시켜줌
            Parallel.Invoke(WhoAmI1, WhoAmI1, WhoAmI1, WhoAmI1, WhoAmI1, WhoAmI1, WhoAmI1, WhoAmI1);

            // Value 값을 지워준다.
            ThreadName.Dispose();
        }
    }
}
