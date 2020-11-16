using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Core;

namespace HslCommunication_Net45.Test.Documentation.Samples.Core
{
    public class ThreadLockExample
    {

        #region SimpleHybirdLockExample1

        private SimpleHybirdLock simpleHybird = new SimpleHybirdLock( );

        public void SimpleHybirdLockExample( )
        {

            // 同步锁，简单的使用
            simpleHybird.Enter( );

            // do something


            simpleHybird.Leave( );

        }

        public void SimpleHybirdLockExample2( )
        {
            // 高级应用，锁的中间是不允许有异常发生的，假如方法会发生异常

            simpleHybird.Enter( );
            try
            {
                int i = 0;
                int j = 6 / i;
                simpleHybird.Leave( );
            }
            catch
            {
                simpleHybird.Leave( );
                throw;
            }

            // 这样做的好处是既没有吞噬异常，锁又安全的离开了
        }


        #endregion

    }
}
