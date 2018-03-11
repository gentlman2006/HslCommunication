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
     *    时间：2017年11月20日 11:43:57
     *    更新：废除原先的2个结果派生类，新增10个泛型派生类，来满足绝大多数的场景使用
     *    
     *    时间：2018年3月11日 22:08:08
     *    更新：新增一些静态方法来方便的获取带有参数的成功对象，新增快速复制错误信息的方法
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


        /// <summary>
        /// 从另一个结果类中拷贝错误信息
        /// </summary>
        /// <typeparam name="TResult">支持结果类及派生类</typeparam>
        /// <param name="result">结果类及派生类的对象</param>
        public void CopyErrorFromOther<TResult>(TResult result) where TResult : OperateResult
        {
            if (result != null)
            {
                ErrorCode = result.ErrorCode;
                CustomerCode = result.CustomerCode;
                Message = result.Message;
                Tag = result.Tag;
            }
            
        }

        #region Static Method


        /// <summary>
        /// 创建并返回一个成功的结果对象
        /// </summary>
        /// <returns>成功的结果对象</returns>
        public static OperateResult CreateSuccessResult()
        {
            return new OperateResult()
            {
                IsSuccess = true,
                Message = StringResources.SuccessText,
            };
        }

        /// <summary>
        /// 创建并返回一个成功的结果对象，并带有一个参数对象
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">类型的值对象</param>
        /// <returns>成功的结果对象</returns>
        public static OperateResult<T> CreateSuccessResult<T>(T value)
        {
            return new OperateResult<T>()
            {
                IsSuccess = true,
                Message = StringResources.SuccessText,
                Content = value
            };
        }


        /// <summary>
        /// 创建并返回一个成功的结果对象，并带有两个参数对象
        /// </summary>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <param name="value1">类型一对象</param>
        /// <param name="value2">类型二对象</param>
        /// <returns>成的结果对象</returns>
        public static OperateResult<T1, T2> CreateSuccessResult<T1, T2>(T1 value1, T2 value2)
        {
            return new OperateResult<T1, T2>()
            {
                IsSuccess = true,
                Message = StringResources.SuccessText,
                Content1 = value1,
                Content2 = value2,
            };
        }


        /// <summary>
        /// 创建并返回一个成功的结果对象，并带有三个参数对象
        /// </summary>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <typeparam name="T3">第三个参数类型</typeparam>
        /// <param name="value1">类型一对象</param>
        /// <param name="value2">类型二对象</param>
        /// <param name="value3">类型三对象</param>
        /// <returns>成的结果对象</returns>
        public static OperateResult<T1, T2, T3> CreateSuccessResult<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            return new OperateResult<T1, T2, T3>()
            {
                IsSuccess = true,
                Message = StringResources.SuccessText,
                Content1 = value1,
                Content2 = value2,
                Content3 = value3,
            };
        }

        /// <summary>
        /// 创建并返回一个成功的结果对象，并带有四个参数对象
        /// </summary>
        /// <typeparam name="T1">第一个参数类型</typeparam>
        /// <typeparam name="T2">第二个参数类型</typeparam>
        /// <typeparam name="T3">第三个参数类型</typeparam>
        /// <typeparam name="T4">第四个参数类型</typeparam>
        /// <param name="value1">类型一对象</param>
        /// <param name="value2">类型二对象</param>
        /// <param name="value3">类型三对象</param>
        /// <param name="value4">类型四对象</param>
        /// <returns>成的结果对象</returns>
        public static OperateResult<T1, T2, T3, T4> CreateSuccessResult<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return new OperateResult<T1, T2, T3, T4>()
            {
                IsSuccess = true,
                Message = StringResources.SuccessText,
                Content1 = value1,
                Content2 = value2,
                Content3 = value3,
                Content4 = value4,
            };
        }

        #endregion

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

    /// <summary>
    /// 操作结果的泛型类，允许带两个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    public class OperateResult<T1, T2> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
    }

    /// <summary>
    /// 操作结果的泛型类，允许带三个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    public class OperateResult<T1, T2, T3> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }
    }


    /// <summary>
    /// 操作结果的泛型类，允许带四个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }
    }


    /// <summary>
    /// 操作结果的泛型类，允许带五个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
    }

    /// <summary>
    /// 操作结果的泛型类，允许带六个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T6 Content6 { get; set; }
        
    }

    /// <summary>
    /// 操作结果的泛型类，允许带七个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    /// <typeparam name="T7">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6, T7> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据6
        /// </summary>
        public T6 Content6 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据7
        /// </summary>
        public T7 Content7 { get; set; }


    }

    /// <summary>
    /// 操作结果的泛型类，允许带八个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    /// <typeparam name="T7">泛型类</typeparam>
    /// <typeparam name="T8">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据6
        /// </summary>
        public T6 Content6 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据7
        /// </summary>
        public T7 Content7 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据8
        /// </summary>
        public T8 Content8 { get; set; }
    }

    /// <summary>
    /// 操作结果的泛型类，允许带九个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    /// <typeparam name="T7">泛型类</typeparam>
    /// <typeparam name="T8">泛型类</typeparam>
    /// <typeparam name="T9">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据6
        /// </summary>
        public T6 Content6 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据7
        /// </summary>
        public T7 Content7 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据8
        /// </summary>
        public T8 Content8 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据9
        /// </summary>
        public T9 Content9 { get; set; }
    }

    /// <summary>
    /// 操作结果的泛型类，允许带十个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    /// <typeparam name="T7">泛型类</typeparam>
    /// <typeparam name="T8">泛型类</typeparam>
    /// <typeparam name="T9">泛型类</typeparam>
    /// <typeparam name="T10">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : OperateResult
    {
        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Content1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Content2 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据3
        /// </summary>
        public T3 Content3 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据4
        /// </summary>
        public T4 Content4 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据5
        /// </summary>
        public T5 Content5 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据6
        /// </summary>
        public T6 Content6 { get; set; }
        
        /// <summary>
        /// 用户自定义的泛型数据7
        /// </summary>
        public T7 Content7 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据8
        /// </summary>
        public T8 Content8 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据9
        /// </summary>
        public T9 Content9 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据10
        /// </summary>
        public T10 Content10 { get; set; }
    }



}
