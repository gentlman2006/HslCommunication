package HslCommunication.Core.Transfer;

/**
 * 应用于多字节数据的解析或是生成格式
 */
public enum DataFormat {
    /**
     * 按照顺序的格式生成的解析规则
     */
    ABCD,
    /**
     * 按照单字反转
     */
    BADC,
    /**
     * 按照双字反转
     */
    CDAB,
    /**
     * 按照倒序排序
     */
    DCBA
}
