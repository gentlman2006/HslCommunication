package HslCommunication.BasicFramework;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * 一个简单的不持久化的序号自增类，采用线程安全实现，并允许指定最大数字，到达后清空从指定数开始
 */
public class SoftIncrementCount {


    /**
     * 实例化一个自增信息的对象，包括最大值
     * @param max 数据的最大值，必须指定
     * @param start 数据的起始值，默认为0
     */
    public SoftIncrementCount( long max, long start )
    {
        this.start = start;
        this.max = max;
        current = start;
        hybirdLock = new ReentrantLock();
    }


    private long start = 0;
    private long current = 0;
    private long max = Long.MAX_VALUE;
    private Lock hybirdLock;


    /**
     * 获取自增信息
     * @return 值
     */
    public long GetCurrentValue( )
    {
        long value = 0;
        hybirdLock.lock( );

        value = current;
        current++;
        if (current > max)
        {
            current = 0;
        }

        hybirdLock.unlock( );
        return value;
    }

}
