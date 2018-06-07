package HslCommunication.LogNet.Core;

/**
 * 日志的存储等级
 */
public enum HslMessageDegree {
    /**
     * 一条消息都不记录
     */
    None,
    /**
     * 记录致命等级及以上日志的消息
     */
    FATAL,
    /**
     *  记录异常等级及以上日志的消息
     */
    ERROR,
    /**
     *  记录警告等级及以上日志的消息
     */
    WARN,
    /**
     *  记录信息等级及以上日志的消息
     */
    INFO,
    /**
     *  记录调试等级及以上日志的信息
     */
    DEBUG
}
