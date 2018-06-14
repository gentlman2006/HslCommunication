package HslCommunication.Core.Types;

/**
 * 带2个参数的结果类
 * @param <T1>
 * @param <T2>
 */
public class OperateResultExTwo<T1,T2> extends OperateResult
{

    /**
     * 泛型对象1
     */
    public T1 Content1 = null;


    /**
     * 泛型对象二
     */
    public T2 Content2 = null;

    /**
     * 创建一个成功的泛型类结果对象
     * @param content1 内容一
     * @param content2 内容二
     * @param <T1> 类型一
     * @param <T2> 类型二
     * @return 结果类对象
     */
    public static <T1,T2> OperateResultExTwo<T1,T2> CreateSuccessResult(T1 content1,T2 content2){
        OperateResultExTwo<T1,T2> result = new OperateResultExTwo<T1,T2>();
        result.IsSuccess = true;
        result.Content1 = content1;
        result.Content2 = content2;
        result.Message = "success";
        return result;
    }

    /**
     * 创建一个失败的泛型类结果对象
     * @param result 复制的结果对象
     * @param <T1> 类型一
     * @param <T2> 类型二
     * @return 结果类对象
     */
    public static <T1,T2> OperateResultExTwo<T1,T2> CreateFailedResult(OperateResult result){
        OperateResultExTwo resultExTwo = new OperateResultExTwo();
        resultExTwo.CopyErrorFromOther(result);
        return resultExTwo;
    }

}
