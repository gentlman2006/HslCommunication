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
