using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / 캐시 이론
    class Thread03
    {
        static void Main_temp(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[y, x] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
                // 약 240만
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                {
                    for (int x = 0; x < 10000; x++)
                    {
                        arr[x, y] = 1;
                    }
                }
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
                // 약 500만
            }

            //인접한 메모리를 캐싱하기 때문에 가까운 메모리 순서로 접근할 시에 눈에 띄는 속도 차이를 보여준다.
        }
    }
}
