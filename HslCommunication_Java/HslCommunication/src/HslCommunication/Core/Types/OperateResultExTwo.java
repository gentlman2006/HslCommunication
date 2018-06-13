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
     * 创建泛型类对象
     * @param content1
     * @param content2
     * @param <T1>
     * @param <T2>
     * @return
     */
    public static <T1,T2> OperateResultExTwo<T1,T2> CreateSuccessResult(T1 content1,T2 content2){
        OperateResultExTwo<T1,T2> resultExTwo = new OperateResultExTwo<T1,T2>();
        resultExTwo.Content1 = content1;
        resultExTwo.Content2 = content2;
        resultExTwo.IsSuccess = true;
        resultExTwo.Message = "success";
        return resultExTwo;
    }


}
