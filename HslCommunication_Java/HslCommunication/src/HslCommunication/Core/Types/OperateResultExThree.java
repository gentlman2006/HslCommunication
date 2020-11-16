package HslCommunication.Core.Types;


/**
 * 带三个参数的泛型类对象
 * @param <T1> 参数一
 * @param <T2> 参数二
 * @param <T3> 参数三
 */
public class OperateResultExThree<T1,T2,T3> extends OperateResult
{


    /**
     * 默认的无参构造方法
     */
    public OperateResultExThree(){
        super();
    }

    /**
     * 使用指定的消息实例化默认的对象
     * @param msg 错误消息
     */
    public OperateResultExThree(String msg){
        super(msg);
    }

    /**
     * 使用指定的错误号和消息实例化默认的对象
     * @param err 错误码
     * @param msg 错误消息
     */
    public OperateResultExThree(int err,String msg){
        super(err,msg);
    }



    /**
     * 泛型对象1
     */
    public T1 Content1 = null;


    /**
     * 泛型对象二
     */
    public T2 Content2 = null;


    /**
     * 泛型对象三
     */
    public T3 Content3 = null;



    /**
     * 创建一个成功的泛型类结果对象
     * @param content1 内容一
     * @param content2 内容二
     * @param content3 内容三
     * @param <T1> 类型一
     * @param <T2> 类型二
     * @param <T3> 类型三
     * @return 结果类对象
     */
    public static <T1,T2,T3> OperateResultExThree<T1,T2,T3> CreateSuccessResult(T1 content1,T2 content2,T3 content3){
        OperateResultExThree<T1,T2,T3> result = new OperateResultExThree<>();
        result.IsSuccess = true;
        result.Content1 = content1;
        result.Content2 = content2;
        result.Content3 = content3;
        result.Message = "success";
        return result;
    }



    /**
     * 创建一个失败的泛型类结果对象
     * @param result 复制的结果对象
     * @param <T1> 类型一
     * @param <T2> 类型二
     * @param <T3> 类型三
     * @return 结果类对象
     */
    public static <T1,T2,T3> OperateResultExThree<T1,T2,T3> CreateFailedResult(OperateResult result){
        OperateResultExThree resultExThree = new OperateResultExThree();
        resultExThree.CopyErrorFromOther(result);
        return resultExThree;
    }
}
