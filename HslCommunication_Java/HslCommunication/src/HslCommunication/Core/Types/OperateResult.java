package HslCommunication.Core.Types;

import HslCommunication.StringResources;

/**
 * 一个结果操作类的基类
 */
public class OperateResult {


    /**
     * 默认的无参构造方法
     */
    public OperateResult(){

    }

    /**
     * 使用指定的消息实例化默认的对象
     * @param msg 错误的消息
     */
    public OperateResult(String msg){
        this.Message = msg;
    }

    /**
     * 使用指定的错误号和消息实例化默认的对象
     * @param err 错误码
     * @param msg 错误消息
     */
    public OperateResult(int err,String msg){
        this.ErrorCode = err;
        this.Message = msg;
    }


    /**
     * 指示本次访问是否成功
     */
    public boolean IsSuccess = false;


    /**
     * 具体的错误描述
     */
    public String Message = StringResources.Language.UnknownError();


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


}

