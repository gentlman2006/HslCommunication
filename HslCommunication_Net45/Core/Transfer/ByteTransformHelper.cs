using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core
{
    /// <summary>
    /// 所有数据转换类的静态辅助方法
    /// Static helper method for all data conversion classes
    /// </summary>
    public static class ByteTransformHelper
    {

        /// <summary>
        /// 结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result">源</param>
        /// <param name="translator">实际转换的委托</param>
        /// <returns>转换结果</returns>
        public static OperateResult<TResult> GetResultFromBytes<TResult>( OperateResult<byte[]> result, Func<byte[], TResult> translator )
        {
            try
            {
                if (result.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult(translator( result.Content ));
                }
                else
                {
                    return OperateResult.CreateFailedResult<TResult>( result );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<TResult>( ) { Message = StringResources.Language.DataTransformError + BasicFramework.SoftBasic.ByteToHexString( result.Content ) + $" : Length({result.Content.Length}) " + ex.Message };
            }
        }

        /// <summary>
        /// 结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result">源结果</param>
        /// <returns>转换结果</returns>
        public static OperateResult<TResult> GetResultFromArray<TResult>( OperateResult<TResult[]> result )
        {
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<TResult>( result );

            return OperateResult.CreateSuccessResult( result.Content[0] );
        }
        
    }
}
