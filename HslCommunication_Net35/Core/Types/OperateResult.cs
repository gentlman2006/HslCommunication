using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication
{

    /*******************************************************************************
     * 
     *    用户返回多个结果数据的一个类，允许返回操作结果，文本信息，错误代号，等等
     *
     *    Used to the return result class in the synchronize communication and communication for industrial Ethernet
     * 
     *******************************************************************************/

        
    /// <summary>
    /// 操作结果的类，只带有成功标志和错误信息
    /// </summary>
    public class OperateResult
    {
        /// <summary>
        /// 指示本次访问是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        

        /// <summary>
        /// 具体的错误描述
        /// </summary>
        public string Message { get; set; } = StringResources.UnknownError;


        /// <summary>
        /// 具体的错误代码
        /// </summary>
        public int ErrorCode { get; set; } = 10000;

        /// <summary>
        /// 允许用户自己使用的一个额外的int数据，可以根据自身的需求进行扩充
        /// </summary>
        public int CustomerCode { get; set; } = 0;


        /// <summary>
        /// 消息附带的额外信息
        /// </summary>
        public object Tag { get; set; }


        /// <summary>
        /// 获取错误代号及文本描述
        /// </summary>
        /// <returns></returns>
        public string ToMessageShowString()
        {
            return $"{StringResources.ErrorCode}:{ErrorCode}{Environment.NewLine}{StringResources.TextDescription}:{Message}";
        }


    }



    /// <summary>
    /// 操作结果的类，除了带有成功标志和错误信息，还带有一个字符串数据
    /// </summary>
    public class OperateResultString : OperateResult
    {
        /// <summary>
        /// 实际的数据
        /// </summary>
        public string Content { get; set; }
    }
    /// <summary>
    /// 操作结果的类，除了带有成功标志和错误信息，还带有一个字节数据
    /// </summary>
    public class OperateResultBytes : OperateResult
    {
        /// <summary>
        /// 实际的数据
        /// </summary>
        public byte[] Content { get; set; }
    }


    /// <summary>
    /// 操作结果的泛型类，允许带一个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T">泛型类</typeparam>
    public class OperateResult<T> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据
        /// </summary>
        public T Content { get; set; }
    }
}
