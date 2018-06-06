package HslCommunication.Core.Types;

import HslCommunication.StringResources;

public class OperateResult
{

    /// <summary>
    /// 指示本次访问是否成功
    /// </summary>
    public boolean IsSuccess = false;

    /// <summary>
    /// 具体的错误描述
    /// </summary>
    public String Message  = "Unknown Errors";

    /// <summary>
    /// 具体的错误代码
    /// </summary>
    public int ErrorCode  = 10000;

    /// <summary>
    /// 获取错误代号及文本描述
    /// </summary>
    /// <returns></returns>
    public String ToMessageShowString()
    {
        return "错误代码："+ErrorCode +"\r\n错误信息："+Message;
    }


    /// <summary>
    /// 从另一个结果类中拷贝错误信息
    /// </summary>
    /// <typeparam name="TResult">支持结果类及派生类</typeparam>
    /// <param name="result">结果类及派生类的对象</param>
    public void CopyErrorFromOther(OperateResult result)
    {
        if (result != null)
        {
            ErrorCode = result.ErrorCode;
            Message = result.Message;
        }

    }
}

public class OperateResult<T1,T2> extends OperateResult{

}