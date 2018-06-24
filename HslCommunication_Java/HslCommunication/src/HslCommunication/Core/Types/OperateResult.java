package HslCommunication.Core.Types;

import HslCommunication.StringResources;

/**
 * 一个结果操作类的基类
 */
public class OperateResult {

    /**
     * 指示本次访问是否成功
     */
    public boolean IsSuccess = false;


    /**
     * 具体的错误描述
     */
    public String Message = StringResources.UnknownError;


    /**
     * 具体的错误代码
     */
    public int ErrorCode = 10000;


    /**
     * @return 获取错误代号及文本描述
     */
    public String ToMessageShowString() {
        return "错误代码：" + ErrorCode + "\r\n错误信息：" + Message;
    }


    /**
     * 从另一个结果类中拷贝错误信息
     *
     * @param result 支持结果类及派生类
     */
    public void CopyErrorFromOther(OperateResult result) {
        if (result != null) {
            ErrorCode = result.ErrorCode;
            Message = result.Message;
        }

    }


    /**
     * 创建一个成功的结果类对象
     *
     * @return 结果类对象
     */
    public static OperateResult CreateSuccessResult() {
        OperateResult result = new OperateResult();
        result.IsSuccess = true;
        result.Message = "success";
        return result;
    }


    /**
     * 创建一个特定错误的结果类对象
     *
     * @param message 错误消息
     * @return 结果类对象
     */
    public static OperateResult CreateFailedResult(String message) {
        OperateResult result = new OperateResult();
        result.Message = message;
        return result;
    }

    /**
     * 创建一个特定错误的结果类对象
     * @param errorCode 错误码
     * @param message 错误消息
     * @return 结果类对象
     */
    public static OperateResult CreateFailedResult(int errorCode, String message) {
        OperateResult result = new OperateResult();
        result.ErrorCode = errorCode;
        result.Message = message;
        return result;
    }


}

