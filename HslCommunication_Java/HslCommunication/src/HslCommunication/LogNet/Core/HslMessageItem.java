package HslCommunication.LogNet.Core;

import HslCommunication.Utilities;

import java.util.Date;
import java.util.concurrent.atomic.AtomicInteger;

/**
 * 单个日志的记录信息
 */
public class HslMessageItem {

    private AtomicInteger IdNumber = new AtomicInteger();
    private long id =0;
    private HslMessageDegree degree = HslMessageDegree.DEBUG;
    private int threadId = 0;
    private String text = "";
    private Date time = new Date();
    private String keyWord = "";

    /**
     * 默认的无参构造器
     */
    public HslMessageItem()
    {
        id = IdNumber.getAndIncrement();
    }


    /**
     * 单个记录信息的标识ID，程序重新运行时清空
     * @return long数据类型
     */
    public long getId() {
        return id;
    }


    /**
     * 获取消息的等级
     * @return 消息等级
     */
    public HslMessageDegree getDegree() {
        return degree;
    }

    /**
     * 设置消息的等级
     * @param degree 消息等级
     */
    public void setDegree(HslMessageDegree degree) {
        this.degree = degree;
    }


    /**
     * 获取线程的标识
     * @return 线程id
     */
    public int getThreadId() {
        return threadId;
    }

    /**
     * 设置线程的标识
     * @param threadId 线程id
     */
    public void setThreadId(int threadId) {
        this.threadId = threadId;
    }

    /**
     * 获取消息文本
     * @return string类型数据
     */
    public String getText() {
        return text;
    }

    /**
     * 设置消息文本
     * @param text 消息文本
     */
    public void setText(String text) {
        this.text = text;
    }

    /**
     * 获取当前的时间信息
     * @return 时间类型
     */
    public Date getTime() {
        return time;
    }

    /**
     * 设置当前的时间
     * @param time 时间
     */
    public void setTime(Date time) {
        this.time = time;
    }

    /**
     * 获取当前的关键字
     * @return string类型的关键字
     */
    public String getKeyWord() {
        return keyWord;
    }

    /**
     * 设置当前的关键字
     * @param keyWord 关键字
     */
    public void setKeyWord(String keyWord) {
        this.keyWord = keyWord;
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串信息
     */
    @Override
    public String toString( )
    {
        if (keyWord == null || keyWord.length() == 0)
        {
            return "["+degree.toString()+"] "+ Utilities.getStringDateShort(time,"yyyy-MM-dd HH:mm:ss.fff") + " Thread["+String.format("D2",threadId)+"] "+text;
        }
        else
        {
            return "["+degree.toString()+"] "+ Utilities.getStringDateShort(time,"yyyy-MM-dd HH:mm:ss.fff") + " Thread["+String.format("D2",threadId)+"] " + keyWord +" : "+text;
        }
    }


    /**
     * 返回表示当前对象的字符串，剔除了关键字
     * @return 字符串数据
     */
    public String ToStringWithoutKeyword()
    {
        return "["+degree.toString()+"] "+ Utilities.getStringDateShort(time,"yyyy-MM-dd HH:mm:ss.fff") + " Thread["+String.format("D2",threadId)+"] "+text;
    }
}
