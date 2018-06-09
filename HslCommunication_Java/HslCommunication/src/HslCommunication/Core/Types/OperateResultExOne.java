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



    public static <T> OperateResultExOne CreateSuccessResult(T content){
        OperateResultExOne<T> resultExOne = new OperateResultExOne<>();
        resultExOne.Content = content;
        resultExOne.IsSuccess = true;
        resultExOne.Message = "success";
        return  resultExOne;
    }

    public static <T> OperateResultExOne CreateFailedResult(OperateResult result){
        OperateResultExOne<T> resultExOne = new OperateResultExOne<>();
        resultExOne.Message = result.Message;
        resultExOne.ErrorCode = result.ErrorCode;
        return  resultExOne;
    }

}
