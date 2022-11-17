using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    //멀티쓰레드 프로그래밍 / ReaderWriterLock
    class Thread10
    {
        class Reward
        {

        }

        //기본적으로는 이게 사용하기도 편하고 많이 쓰일 것이다.
        static object _lock1 = new object();

        //Slim이 최신버전
        static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        //99.9999%의 경우로 이 함수만 사용해서 보상을 가져옴
        static Reward GetRewardById(int id)
        {
            //ReadLock끼리는 서로 잠구지않고 Lock이 없는 것처럼 진행됨
            _lock.EnterReadLock();

            _lock.ExitReadLock();
            return null;
        }

        //0.00001%의 확률로 가~끔 보상을 늘림
        static void AddReward(Reward reward)
        {
            //WriteLock을 잠그는 순간 Read도 잠겨서 상호 배제가 시작됨
            _lock.EnterWriteLock();

            _lock.ExitWriteLock();
        }

        static void Main_temp(string[] args)
        {

        }
    }
}
