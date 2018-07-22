using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace HslCommunication.BasicFramework
{



    /****************************************************************************
     * 
     *    创建日期： 2017年6月25日 15:45:40
     *    功能：      一个基础的泛型异常类
     *    参考：      参考《CLR Via C#》P413
     * 
     ***************************************************************************/
    /// <summary>
    /// 一个自定义的支持序列化反序列化的异常类，具体用法参照第四版《CLR Via C#》P414
    /// </summary>
    /// <typeparam name="TExceptionArgs">泛型异常</typeparam>
    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable where TExceptionArgs : ExceptionArgs
    {

        /// <summary>
        /// 用于反序列化的
        /// </summary>
        private const string c_args = "Args";

        private readonly TExceptionArgs m_args;

        /// <summary>
        /// 消息
        /// </summary>
        public TExceptionArgs Args { get { return m_args; } }


        /// <summary>
        /// 实例化一个异常对象
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常类</param>
        public Exception(string message = null, Exception innerException = null) : this(null, message, innerException)
        {

        }

        /// <summary>
        /// 实例化一个异常对象
        /// </summary>
        /// <param name="args">异常消息</param>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常类</param>
        public Exception(TExceptionArgs args, string message = null, Exception innerException = null) : base(message, innerException)
        {
            m_args = args;
        }


        /******************************************************************************************************
         * 
         *    这个构造器用于反序列化的，由于类是密封的，所以构造器是私有的
         *    如果这个构造器不是密封的，这个构造器就应该是受保护的
         * 
         ******************************************************************************************************/
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_args = (TExceptionArgs)info.GetValue(c_args, typeof(TExceptionArgs));
        }

        /******************************************************************************************************
         * 
         *    这个方法用于序列化，由于ISerializable接口的存在，这个方法必须是公开的
         * 
         ******************************************************************************************************/


        /// <summary>
        /// 获取存储对象的序列化数据
        /// </summary>
        /// <param name="info">序列化的信息</param>
        /// <param name="context">流的上下文</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(c_args, m_args);
            base.GetObjectData(info, context);
        }


        /// <summary>
        /// 获取描述当前异常的消息
        /// </summary>
        public override string Message
        {
            get
            {
                string baseMsg = base.Message;
                return m_args == null ? baseMsg : baseMsg + " (" + m_args.Message + ")";
            }
        }


        /// <summary>
        /// 确定指定的object是否等于当前的object
        /// </summary>
        /// <param name="obj">异常对象</param>
        /// <returns>是否一致</returns>
        public override bool Equals(object obj)
        {
            Exception<TExceptionArgs> other = obj as Exception<TExceptionArgs>;
            if (other == null) return false;
            return object.Equals(m_args, other.m_args) && base.Equals(obj);
        }


        /// <summary>
        /// 用作特定类型的哈希函数
        /// </summary>
        /// <returns>int值</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    /// 异常消息基类
    /// </summary>
    [Serializable]
    public abstract class ExceptionArgs
    {
        /// <summary>
        /// 获取消息文本
        /// </summary>
        public virtual string Message
        {
            get { return string.Empty; }
        }
    }

}
