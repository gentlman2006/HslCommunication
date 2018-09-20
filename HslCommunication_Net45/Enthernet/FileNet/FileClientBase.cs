using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 文件传输客户端基类，提供上传，下载，删除的基础服务
    /// </summary>
    public abstract class FileClientBase : NetworkXBase
    {

        #region Private Member

        private IPEndPoint m_ipEndPoint = null;

        #endregion

        #region Public Member

        /// <summary>
        /// 服务器端的文件管理引擎终结点
        /// </summary>
        public IPEndPoint ServerIpEndPoint
        {
            get { return m_ipEndPoint; }
            set { m_ipEndPoint = value; }
        }

        /// <summary>
        /// 获取或设置连接的超时时间，默认10秒
        /// </summary>
        public int ConnectTimeOut { get; set; } = 10000;

        #endregion

        #region Private Method

        /// <summary>
        /// 发送三个文件分类到服务器端
        /// </summary>
        /// <param name="socket">套接字对象</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult SendFactoryGroupId(
            Socket socket,
            string factory,
            string group,
            string id
            )
        {
            OperateResult factoryResult = SendStringAndCheckReceive( socket, 1, factory );
            if (!factoryResult.IsSuccess)
            {
                return factoryResult;
            }

            OperateResult groupResult = SendStringAndCheckReceive( socket, 2, group );
            if (!groupResult.IsSuccess)
            {
                return groupResult;
            }

            OperateResult idResult = SendStringAndCheckReceive( socket, 3, id );
            if (!idResult.IsSuccess)
            {
                return idResult;
            }

            return OperateResult.CreateSuccessResult( ); ;
        }


        #endregion

        #region Delete File

        /// <summary>
        /// 删除服务器上的文件
        /// </summary>
        /// <param name="fileName">文件的名称</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DeleteFileBase( string fileName, string factory, string group, string id )
        {
            // connect server
            OperateResult<Socket> socketResult = CreateSocketAndConnect( ServerIpEndPoint, ConnectTimeOut );
            if (!socketResult.IsSuccess) return socketResult;


            // 发送操作指令
            OperateResult sendString = SendStringAndCheckReceive( socketResult.Content, HslProtocol.ProtocolFileDelete, fileName );
            if (!sendString.IsSuccess) return sendString;

            // 发送文件名以及三级分类信息
            OperateResult sendFileInfo = SendFactoryGroupId( socketResult.Content, factory, group, id );
            if (!sendFileInfo.IsSuccess) return sendFileInfo;

            // 接收服务器操作结果
            OperateResult<int, string> receiveBack = ReceiveStringContentFromSocket( socketResult.Content );
            if (!receiveBack.IsSuccess) return receiveBack;

            OperateResult result = new OperateResult( );

            if (receiveBack.Content1 == 1) result.IsSuccess = true;
            result.Message = receiveBack.Message;

            socketResult.Content?.Close( );
            return result;
        }

        #endregion

        #region Download File

        /// <summary>
        /// 基础下载信息
        /// </summary>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <param name="fileName">服务器的文件名称</param>
        /// <param name="processReport">下载的进度报告</param>
        /// <param name="source">数据源信息，决定最终存储到哪里去</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DownloadFileBase(
            string factory,
            string group,
            string id,
            string fileName,
            Action<long, long> processReport,
            object source
            )
        {
            // connect server
            OperateResult<Socket> socketResult = CreateSocketAndConnect( ServerIpEndPoint, ConnectTimeOut );
            if (!socketResult.IsSuccess) return socketResult;

            // 发送操作指令
            OperateResult sendString = SendStringAndCheckReceive( socketResult.Content, HslProtocol.ProtocolFileDownload, fileName );
            if (!sendString.IsSuccess) return sendString;

            // 发送三级分类
            OperateResult sendClass = SendFactoryGroupId( socketResult.Content, factory, group, id );
            if (!sendClass.IsSuccess) return sendClass;
           

            // 根据数据源分析
            if (source is string fileSaveName)
            {
                OperateResult result = ReceiveFileFromSocket( socketResult.Content, fileSaveName, processReport );
                if (!result.IsSuccess) return result;
            }
            else if (source is Stream stream)
            {
                OperateResult result = ReceiveFileFromSocket( socketResult.Content, stream, processReport );
                if (!result.IsSuccess)
                {
                    return result;
                }
            }
            else
            {
                socketResult.Content?.Close( );
                LogNet?.WriteError( ToString(), StringResources.Language.NotSupportedDataType );
                return new OperateResult( StringResources.Language.NotSupportedDataType );
            }

            socketResult.Content?.Close( );
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Upload File


        /// <summary>
        /// 上传文件给服务器
        /// </summary>
        /// <param name="source">数据源，可以是文件名，也可以是数据流</param>
        /// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <param name="fileTag">文件的描述</param>
        /// <param name="fileUpload">文件的上传人</param>
        /// <param name="processReport">汇报进度</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult UploadFileBase(
            object source,
            string serverName,
            string factory,
            string group,
            string id,
            string fileTag,
            string fileUpload,
            Action<long, long> processReport )
        {
            // HslReadWriteLock readWriteLock = new HslReadWriteLock( );
            

            // 创建套接字并连接服务器
            OperateResult<Socket> socketResult = CreateSocketAndConnect( ServerIpEndPoint, ConnectTimeOut );
            if (!socketResult.IsSuccess) return socketResult;
            
            // 上传操作暗号的文件名
            OperateResult sendString = SendStringAndCheckReceive( socketResult.Content, HslProtocol.ProtocolFileUpload, serverName );
            if (!sendString.IsSuccess) return sendString;
            
            // 发送三级分类
            OperateResult sendClass = SendFactoryGroupId( socketResult.Content, factory, group, id );
            if (!sendClass.IsSuccess) return sendClass;

            // 判断数据源格式
            if (source is string fileName)
            {
                OperateResult result = SendFileAndCheckReceive( socketResult.Content, fileName, serverName, fileTag, fileUpload, processReport );
                if(!result.IsSuccess)
                {
                    return result;
                }
            }
            else if (source is Stream stream)
            {
                OperateResult result = SendFileAndCheckReceive( socketResult.Content, stream, serverName, fileTag, fileUpload, processReport );
                if (!result.IsSuccess)
                {
                    return result;
                }
            }
            else
            {
                socketResult.Content?.Close( );
                LogNet?.WriteError( ToString( ), StringResources.Language.DataSourseFormatError );
                return new OperateResult( StringResources.Language.DataSourseFormatError );
            }
            

            // 确认服务器文件保存状态
            OperateResult<int, string> resultCheck = ReceiveStringContentFromSocket( socketResult.Content );
            if (!resultCheck.IsSuccess) return resultCheck;
            
            if (resultCheck.Content1 == 1)
            {
                return OperateResult.CreateSuccessResult( );
            }
            else
            {
                return new OperateResult( StringResources.Language.ServerFileCheckFailed );
            }
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return "FileClientBase";
        }

        #endregion
    }
}
