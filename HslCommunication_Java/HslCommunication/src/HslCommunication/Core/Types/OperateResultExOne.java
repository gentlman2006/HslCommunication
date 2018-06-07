package HslCommunication.Core.Types;


/**
 * 带一个参数的结果类对象
 * @param <T>
 */
public class OperateResultExOne<T> extends  OperateResult
{
    /**
     * 泛型参数对象
     */
    public T Content = null;
}
