using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / SpinLock + Context switching

    class SpinLock_fail
    {
        volatile bool _locked = false;

        public void Acquire()
        {
            while (_locked)
            {
                // 잠김이 풀리기를 기다린다
                // 두 개 이상의 쓰레드가 동시에 여기에 들어오게 된다면 둘다 while문을 빠져나가 버린다.
                // 따라서 상호배제를 보장할 수 없다.
            }

            //내꺼!
            _locked = true;
        }

        public void Release()
        {
            _locked = false;
        }
    }

    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquire()
        {
            while (true)
            {
                /*
                int original = Interlocked.Exchange(ref _locked, 1);
                if (original == 0)
                    break;

                멀티쓰레딩을 하다보면 위험한 변수와 그렇지 않은 변수를 잘 구분해야함
                _locked는 공유변수이므로 상태를 보장하지 못하기 때문에 조건문으로 사용하거나 대입해서 사용할 시 위험함
                original은 원자성을 보장하는 Interlocked클래스의 함수로 인해 나온 값이고 스택 변수 즉 하나의 쓰레드에서만 관리하는 변수이므로 안전함

                Interlocked.Exchange 의사코드
                {
                    int original = _locked;
                    _locked = 1;
                }
                */

                // CAS Compare-And-Swap 함수라고 함 C++에도 같은 기능을 하는 함수가 있음
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;
                // 위의 4줄 코드 자체가 스핀락은 아니고 이것은 잠긴 여부를 안전하게 확인하는 코드이다.
                // 잠겨있을 때 while(true)로 인해 계속 확인하는 과정을 spinlock이라고 한다.

                /*
                Interlocked.CompareExchange(ref _locked, desired, expected)의 의사코드
                {
                    int original = _locked;
                    if (_locked == expected)
                        _locked = desired;
                }
                */

                // 밑의 예시는 spinlock이 아닌 잠겨있을 경우 cpu를 잠시 포기하는 방식이다.
                // cpu를 넘길때마다 컨텍스트 스위칭에 의한 비용이 발생하므로 무조건 좋은건 아니고 상황을 봐가야한다.
                /*
                Thread.Sleep(1); //무조건 휴식 => 무조건 1ms 정도 쉬고 싶어요. (1ms는 희망사항일 뿐 운영체제가 알아서 한다)
                Thread.Sleep(0); //조건부 양보 => 나보다 우선순위가 낮은 애들한테는 양보 불가 => 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테
                Thread.Yield();  //관대한 양보 => 관대하게 양보할테니, 지금 실행이 가능한 쓰레드가 있으면 실행하세요 => 실행 가능한 애가 없으면 남은 시간 소진
                */
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    class Thread08
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; ++i)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; ++i)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
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
