package HslCommunication.Core.Types;


/**
 * 带一个参数的结果类对象
 * @param <T>
 */
public class OperateResultExOne<T> extends  OperateResult
{

    /**
     * 默认的无参构造方法
     */
    public OperateResultExOne(){
        super();
    }

    /**
     * 使用指定的消息实例化默认的对象
     * @param msg 错误消息
     */
    public OperateResultExOne(String msg){
        super(msg);
    }

    /**
     * 使用指定的错误号和消息实例化默认的对象
     * @param err 错误码
     * @param msg 错误消息
     */
    public OperateResultExOne(int err,String msg){
        super(err,msg);
    }


    /**
     * 泛型参数对象
     */
    public T Content = null;


    /**
     * 创建一个失败的对象
     * @param result 失败的结果
     * @param <T> 类型参数
     * @return 结果类对象
     */
    public static <T> OperateResultExOne<T> CreateFailedResult(OperateResult result){
        OperateResultExOne<T> resultExOne = new OperateResultExOne<T>();
        resultExOne.CopyErrorFromOther(result);
        return resultExOne;
    }


    /**
     * 创建一个成功的泛型类结果对象
     * @param content 内容
     * @param <T> 类型
     * @return 结果类对象
     */
    public static <T> OperateResultExOne<T> CreateSuccessResult(T content){
        OperateResultExOne<T> result = new OperateResultExOne<T>();
        result.IsSuccess = true;
        result.Content = content;
        result.Message = "success";
        return result;
    }

}
