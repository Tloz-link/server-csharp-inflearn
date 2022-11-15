using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / 쓰레드 생성 강의
    class Thread01
    {
        static void MainThread(object obj)
        {
            for (int i = 0; i < 5; ++i)
                Console.WriteLine("Hello Thread!");
        }

        static void Main_temp(string[] args)
        {
            //StudyThread();
            //StudyThreadPool();
            //StudyTask();
        }

        static void StudyTask()
        {
            //기본적으로는 쓰레드 풀에서 사용하지만 LongRunning옵션을 넣어주면 풀에서 안빼고 따로 혼자 돎
            //Thread 클래스는 거의 쓸일 없고 풀을 쓰거나 차라리 Task를 쓰게 될 것이다.
            //ThreadPool과 Task는 기본적으로 background에서 돈다.
            Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
            t.Start();
        }

        static void StudyThreadPool()
        {
            // 쓰레드 풀을 1~5개로 유지함
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            // 이렇게 5개를 다 써버리면 6번째 쓰레드풀 요청은 받지 못해서 먹통이 됨
            for (int i = 0; i < 5; ++i)
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });

            //관리되고 있는 쓰레드를 반환해주기 때문에 부담이 적음
            //풀링된 스레드 수가 제한되어 있음
            //쓰레드를 무한루프같이 긴 시간동안 잡아두면 풀이 부족해지는 문제 발생
            ThreadPool.QueueUserWorkItem(MainThread);
        }

        static void StudyThread()
        {
            //쓰레드 수를 코어 수와 맞춰주는 것이 이상적
            Thread t = new Thread(MainThread); //쓰레드를 직접 만들면 부담이 있음 (관리 부담)
            t.Name = "Test Thread";            //쓰레드 이름 지정
            t.IsBackground = false;             //기본은 false로 Foreground로 돌고있다(메인 종료시에도 계속 돎)
            t.Start();

            Console.WriteLine("Waiting for Thread!");

            t.Join();
            Console.WriteLine("Hello, World!");
        }
    }
}