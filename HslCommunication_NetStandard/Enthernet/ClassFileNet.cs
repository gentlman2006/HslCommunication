using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using HslCommunication.BasicFramework;
using HslCommunication.LogNet;
using HslCommunication.Core;


namespace HslCommunication.Enthernet
{
    /*****************************************************************************
     * 
     *    用于服务器客户端文件传输的类
     *    目前不支持断点续传，
     *    更新日期：2017-09-12
     *    当文件的数据可能比较大时，将文件信息存储到数据库中是一种很明智的决定
     *    如果文件的内容也存储到数据库中，将不能支持上传或下载的进度显示
     * 
     *****************************************************************************/
     

    /// <summary>
    /// 文件传输客户端基类
    /// </summary>
    public abstract class FileClientBase : NetShareBase
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

        #endregion

        #region Private Method

        /// <summary>
        /// 发送三个文件分类到服务器端
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool SendFactoryGroupId(
            Socket socket,
            string factory,
            string group,
            string id,
            OperateResult result
            )
        {
            if (!SendStringAndCheckReceive(
               socket,
               1,
               factory,
               result,
               null,
               "发送下载文件操作指令异常"))
            {
                return false;
            }

            if (!SendStringAndCheckReceive(
                socket,
                2,
                group,
                result,
                null,
                "发送下载文件操作指令异常"))
            {
                return false;
            }

            if (!SendStringAndCheckReceive(
                socket,
                3,
                id,
                result,
                null,
                "发送下载文件操作指令异常"))
            {
                return false;
            }

            return true;
        }


        #endregion

        #region Delete File

        /// <summary>
        /// 删除服务器上的文件
        /// </summary>
        /// <param name="fileName">文件的名称</param>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected OperateResult DeleteFileBase(string fileName,string factory,string group,string id)
        {
            OperateResult result = new OperateResult();

            // connect server
            if (!CreateSocketAndConnect(out Socket socket, ServerIpEndPoint, result)) return result;

            // 发送操作指令
            if (!SendStringAndCheckReceive(
                socket,
                HslCommunicationCode.Hsl_Protocol_File_Delete,
                fileName,
                result,
                null,
                "发送删除操作指令异常"))
            {
                return result;
            }

            if (!SendFactoryGroupId(
                socket,
                factory,
                group,
                id,
                result))
            {
                return result;
            }

            // 接收服务器操作结果
            if (!ReceiveStringFromSocket(socket, out int operater, out string note, result, null, "接收服务器操作结果指令异常"))
            {
                return result;
            }

            if (operater == 1)
            {
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "接收服务器删除结果异常";
            }

            socket?.Close();

            return result;
        }

        #endregion

        #region Download File

        /// <summary>
        /// 基础下载信息
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <param name="fileName">服务器的文件名称</param>
        /// <param name="processReport">下载的进度报告</param>
        /// <param name="source">数据源信息，决定最终存储到哪里去</param>
        /// <returns></returns>
        protected OperateResult DownloadFileBase(
            string factory, 
            string group, 
            string id, 
            string fileName, 
            Action<long, long> processReport, 
            object source
            )
        {
            OperateResult result = new OperateResult();

            // connect server
            if (!CreateSocketAndConnect(out Socket socket, ServerIpEndPoint, result)) return result;

            // 发送操作指令
            if (!SendStringAndCheckReceive(
                socket,
                HslCommunicationCode.Hsl_Protocol_File_Download,
                fileName,
                result,
                null,
                "发送下载文件操作指令异常"))
            {
                return result;
            }

            if(!SendFactoryGroupId(
                socket,
                factory,
                group,
                id,
                result))
            {
                return result;
            }


            // 根据数据源分析
            if (source is string fileSaveName)
            {
                if (!ReceiveFileFromSocket(
                    socket,
                    fileSaveName,
                    out string filename,
                    out long filesize,
                    out string filetag,
                    out string fileupload,
                    result,
                    processReport,
                    "下载文件的时候发生异常"
                    ))
                {
                    return result;
                }
            }
            else if(source is Stream stream)
            {
                if (!ReceiveFileFromSocket(
                socket,
                stream,
                out string filename,
                out long filesize,
                out string filetag,
                out string fileupload,
                result,
                processReport,
                "下载文件的时候发生异常"
                ))
                {
                    return result;
                }
            }
            else
            {
                socket?.Close();
                LogNet?.WriteError(LogHeaderText,"Not supported data type!");
                return result;
            }



            socket?.Close();

            result.IsSuccess = true;
            return result;
        }

        #endregion

        #region Upload File


        /// <summary>
        /// 上传文件给服务器
        /// </summary>
        /// <param name="source">数据源，可以是文件名，也可以是数据流</param>
        /// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <param name="fileTag">文件的描述</param>
        /// <param name="fileUpload">文件的上传人</param>
        /// <param name="processReport">汇报进度</param>
        /// <returns></returns>
        protected OperateResult UploadFileBase(
            object source,
            string serverName,
            string factory,
            string group,
            string id,
            string fileTag,
            string fileUpload,
            Action<long, long> processReport)
        {
            OperateResult result = new OperateResult();

            HslReadWriteLock readWriteLock = new HslReadWriteLock();

            // 创建套接字并连接服务器
            if (!CreateSocketAndConnect(out Socket socket, ServerIpEndPoint, result)) return result;

            // 上传操作暗号的文件名
            if (!SendStringAndCheckReceive(
                socket,
                HslCommunicationCode.Hsl_Protocol_File_Upload,
                serverName,
                result,
                null,
                "发送上传文件操作指令异常"))
            {
                return result;
            }

            if (!SendFactoryGroupId(
                socket,
                factory,
                group,
                id,
                result))
            {
                return result;
            }

            // 判断数据源格式
            if (source is string fileName)
            {
                if (!SendFileAndCheckReceive(socket, fileName, serverName, fileTag, fileUpload, result, processReport, "向服务器发送文件失败"))
                {
                    return result;
                }
            }
            else if(source is Stream stream)
            {
                if (!SendFileAndCheckReceive(socket, stream, serverName, fileTag, fileUpload, result, processReport, "向服务器发送文件失败"))
                {
                    return result;
                }
            }
            else
            {
                socket?.Close();
                LogNet?.WriteError(LogHeaderText,"source if not corrected!");
                return result;
            }

            // check the server result
            if (!ReceiveStringFromSocket(socket, out int status, out string nosense, result, null, "确认服务器保存状态失败"))
            {
                return result;
            }

            socket?.Close();

            if (status == 1) result.IsSuccess = true;
            else result.Message = "服务器确认文件失败，请重新上传！";

            return result;
        }

        #endregion
    }


    

    /// <summary>
    /// 负责分类文件传输的客户端，用来上传，下载文件信息
    /// </summary>
    public sealed class IntegrationFileClient : FileClientBase
    {
        #region Constructor
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public IntegrationFileClient()
        {
            LogHeaderText = "IntegrationFileClient";
        }

        #endregion

        #region Delete File

        /// <summary>
        /// 删除服务器的文件操作
        /// </summary>
        /// <param name="fileName">文件名称，带后缀</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns></returns>
        public OperateResult DeleteFile(
            string fileName,
            string factory,
            string group,
            string id)
        {
            return DeleteFileBase(fileName, factory, group, id);
        }


        #endregion

        #region Download File


        /// <summary>
        /// 下载服务器的文件到本地的文件操作
        /// </summary>
        /// <param name="fileName">文件名称，带后缀</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="processReport">下载的进度报告</param>
        /// <param name="fileSaveName">准备本地保存的名称</param>
        /// <returns></returns>
        public OperateResult DownloadFile(
            string fileName,
            string factory,
            string group,
            string id,
            Action<long, long> processReport,
            string fileSaveName
            )
        {
            return DownloadFileBase(factory, group, id, fileName, processReport, fileSaveName);
        }

        /// <summary>
        /// 下载服务器的文件到本地的数据流中
        /// </summary>
        /// <param name="fileName">文件名称，带后缀</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="processReport">下载的进度报告</param>
        /// <param name="stream">流数据</param>
        /// <returns></returns>
        public OperateResult DownloadFile(
            string fileName,
            string factory,
            string group,
            string id,
            Action<long, long> processReport,
            Stream stream
            )
        {
            return DownloadFileBase(factory, group, id, fileName, processReport, stream);
        }
        


        #endregion

        #region Upload File

        /// <summary>
        /// 上传本地的文件到服务器操作
        /// </summary>
        /// <param name="fileName">本地的完整路径的文件名称</param>
        /// <param name="serverName">服务器存储的文件名称，带后缀</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileTag">文件的额外描述</param>
        /// <param name="fileUpload">文件的上传人</param>
        /// <param name="processReport">上传的进度报告</param>
        /// <returns></returns>
        public OperateResult UploadFile(
            string fileName,
            string serverName,
            string factory,
            string group,
            string id,
            string fileTag,
            string fileUpload,
            Action<long, long> processReport)
        {
            return UploadFileBase(fileName, serverName, factory, group, id, fileTag, fileUpload, processReport);
        }

        /// <summary>
        /// 上传数据流到服务器操作
        /// </summary>
        /// <param name="stream">数据流内容</param>
        /// <param name="serverName">服务器存储的文件名称，带后缀</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileTag">文件的额外描述</param>
        /// <param name="fileUpload">文件的上传人</param>
        /// <param name="processReport">上传的进度报告</param>
        /// <returns></returns>
        public OperateResult UploadFile(
            Stream stream,
            string serverName,
            string factory,
            string group,
            string id,
            string fileTag,
            string fileUpload,
            Action<long, long> processReport)
        {
            return UploadFileBase(stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
        }




        #endregion

        #region Private Method

        /// <summary>
        /// 根据三种分类信息，还原成在服务器的相对路径，包含文件
        /// </summary>
        /// <param name="fileName">文件名称，包含后缀名</param>
        /// <param name="factory">第一类</param>
        /// <param name="group">第二类</param>
        /// <param name="id">第三类</param>
        /// <returns></returns>
        private string TranslateFileName(string fileName, string factory, string group, string id)
        {
            string file_save_server_name = fileName;

            if (id.IndexOf('\\') >= 0) id = id.Replace('\\', '_');
            if (group.IndexOf('\\') >= 0) group = id.Replace('\\', '_');
            if (factory.IndexOf('\\') >= 0) id = factory.Replace('\\', '_');


            if (id?.Length > 0) file_save_server_name = id + @"\" + file_save_server_name;

            if (group?.Length > 0) file_save_server_name = group + @"\" + file_save_server_name;

            if (factory?.Length > 0) file_save_server_name = factory + @"\" + file_save_server_name;

            return file_save_server_name;
        }

        /// <summary>
        /// 根据三种分类信息，还原成在服务器的相对路径，仅仅路径
        /// </summary>
        /// <param name="factory">第一类</param>
        /// <param name="group">第二类</param>
        /// <param name="id">第三类</param>
        /// <returns></returns>
        private string TranslatePathName(string factory, string group, string id)
        {
            string file_save_server_name = "";

            if (id.IndexOf('\\') >= 0) id = id.Replace('\\', '_');
            if (group.IndexOf('\\') >= 0) group = id.Replace('\\', '_');
            if (factory.IndexOf('\\') >= 0) id = factory.Replace('\\', '_');

            if (id?.Length > 0) file_save_server_name = @"\" + id;

            if (group?.Length > 0) file_save_server_name = @"\" + group + file_save_server_name;

            if (factory?.Length > 0) file_save_server_name = @"\" + factory + file_save_server_name;

            return file_save_server_name;
        }


        #endregion

        #region Get FileNames

        /// <summary>
        /// 获取指定路径下的所有的文档
        /// </summary>
        /// <param name="fileNames">获取得到的文件合集</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns></returns>
        public OperateResult DownloadPathFileNames(
            out GroupFileItem[] fileNames,
            string factory,
            string group,
            string id
            )
        {
            return DownloadStringArrays(
                out fileNames,
                HslCommunicationCode.Hsl_Protocol_File_Directory_Files,
                factory,
                group,
                id
                );
        }
        

        #endregion

        #region Get FolderNames

        /// <summary>
        /// 获取指定路径下的所有的文档
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="factory"></param>
        /// <param name="group"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult DownloadPathFolders(
            out string[] folders,
            string factory,
            string group,
            string id
            )
        {
            return DownloadStringArrays(
                out folders,
                HslCommunicationCode.Hsl_Protocol_File_Directories,
                factory,
                group,
                id);
        }


        #endregion

        #region Private Method

        /// <summary>
        /// 获取指定路径下的所有的文档
        /// </summary>
        /// <param name="arrays">想要获取的队列</param>
        /// <param name="protocol">指令</param>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <returns></returns>
        private OperateResult DownloadStringArrays<T>(
            out T[] arrays,
            int protocol,
            string factory,
            string group,
            string id
            )
        {
            OperateResult result = new OperateResult();
            // 连接服务器
            if (!CreateSocketAndConnect(out Socket socket, ServerIpEndPoint, result))
            {
                arrays = new T[0];
                return result;
            }
            
            // 上传信息
            if (!SendStringAndCheckReceive(
                socket,
                protocol,
                "nosense",
                result,
                null,
                "发送获取数组操作指令异常"))
            {
                arrays = null;
                return result;
            }

            if (!SendFactoryGroupId(
                socket,
                factory,
                group,
                id,
                result))
            {
                arrays = null;
                return result;
            }


            if (!ReceiveStringFromSocket(socket, out int customer, out string jsonStr, result, null, "接收列表数据异常"))
            {
                arrays = null;
                return result;
            }

            arrays = Newtonsoft.Json.Linq.JArray.Parse(jsonStr).ToObject<T[]>();

            socket?.Close();
            result.IsSuccess = true;
            return result;
        }

        #endregion

    }
    

    /// <summary>
    /// 文件管理类服务器，负责服务器所有分类文件的管理，仅仅负责本地文件的存储
    /// </summary>
    public sealed class AdvancedFileServer : FileServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public AdvancedFileServer()
        {
            LogHeaderText = "AdvancedFileServer";
        }

        #endregion

        #region Override Method

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            if (obj is Socket socket)
            {

                OperateResult result = new OperateResult();

                // 获取ip地址
                string IpAddress = ((IPEndPoint)(socket.RemoteEndPoint)).Address.ToString();

                // 接收操作信息
                if (!ReceiveInformationHead(
                    socket,
                    out int customer,
                    out string fileName,
                    out string Factory,
                    out string Group,
                    out string Identify,
                    result,
                    "filename received failed.ip:" + IpAddress
                    ))
                {
                    return;
                }

                string relativeName = ReturnRelativeFileName(Factory, Group, Identify, fileName);

                // 操作分流

                if (customer == HslCommunicationCode.Hsl_Protocol_File_Download)
                {
                    string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);

                    // 发送文件数据
                    if (!SendFileAndCheckReceive(socket, fullFileName, fileName, "", "", result, null))
                    {
                        LogNet?.WriteError(LogHeaderText, $"{StringResources.FileDownloadFailed}:{relativeName} ip:{IpAddress}");
                        return;
                    }
                    else
                    {
                        LogNet?.WriteInfo(LogHeaderText, StringResources.FileDownloadSuccess + ":" + relativeName);
                    }
                    socket?.Close();
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Upload)
                {
                    string tempFileName = FilesDirectoryPathTemp + "\\" + CreateRandomFileName();

                    string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);

                    // 上传文件
                    CheckFolderAndCreate();

                    try
                    {
                        FileInfo info = new FileInfo(fullFileName);
                        if (!Directory.Exists(info.DirectoryName))
                        {
                            Directory.CreateDirectory(info.DirectoryName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogNet?.WriteException(LogHeaderText,"创建文件夹失败：" + fullFileName, ex);
                        socket?.Close();
                        return;
                    }

                    if (ReceiveFileFromSocketAndMoveFile(
                        socket,                                 // 网络套接字
                        tempFileName,                           // 临时保存文件路径
                        fullFileName,                           // 最终保存文件路径
                        out string FileName,                    // 文件名称，从客户端上传到服务器时，为上传人
                        out long FileSize,
                        out string FileTag,
                        out string FileUpload,
                        result
                        ))
                    {
                        socket?.Close();
                        LogNet?.WriteInfo(LogHeaderText,StringResources.FileUploadSuccess + ":" + relativeName);
                    }
                    else
                    {
                        LogNet?.WriteInfo(LogHeaderText,StringResources.FileUploadFailed + ":" + relativeName);
                    }
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Delete)
                {
                    string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);

                    bool deleteResult = DeleteFileByName(fullFileName);

                    // 回发消息
                    if (SendStringAndCheckReceive(
                        socket,                                                                // 网络套接字
                        deleteResult ? 1 : 0,                                                  // 是否移动成功
                        deleteResult ? "成功" : "失败",                                        // 字符串数据
                        result,                                                                // 结果数据对象
                        null,                                                                  // 不进行报告
                        "回发删除结果错误")                                                    // 发送错误时的数据
                        )
                    {
                        socket?.Close();
                    }

                    if (deleteResult) LogNet?.WriteInfo(LogHeaderText,StringResources.FileDeleteSuccess + ":" + fullFileName);
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Directory_Files)
                {
                    List<GroupFileItem> fileNames = new List<GroupFileItem>();
                    foreach (var m in GetDirectoryFiles(Factory, Group, Identify))
                    {
                        FileInfo fileInfo = new FileInfo(m);
                        fileNames.Add(new GroupFileItem()
                        {
                            FileName = fileInfo.Name,
                            FileSize = fileInfo.Length,
                        });
                    }

                    Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.FromObject(fileNames.ToArray());
                    if (SendStringAndCheckReceive(
                        socket,
                        HslCommunicationCode.Hsl_Protocol_File_Directory_Files,
                        jArray.ToString(),
                        result,
                        null,
                        "发送文件列表回客户端失败"))
                    {
                        socket?.Close();
                    }
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Directories)
                {
                    List<string> folders = new List<string>();
                    foreach (var m in GetDirectories(Factory, Group, Identify))
                    {
                        DirectoryInfo directory = new DirectoryInfo(m);
                        folders.Add(directory.Name);
                    }

                    Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.FromObject(folders.ToArray());
                    if (SendStringAndCheckReceive(
                        socket,
                        HslCommunicationCode.Hsl_Protocol_File_Directory_Files,
                        jArray.ToString(),
                        result,
                        null,
                        "发送文件夹列表回客户端失败"))
                    {
                        socket?.Close();
                    }
                }
                else
                {
                    socket?.Close();
                }
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected override void StartInitialization()
        {
            if (string.IsNullOrEmpty(FilesDirectoryPathTemp))
            {
                throw new ArgumentNullException("FilesDirectoryPathTemp", "No saved path is specified");
            }

            base.StartInitialization();
        }

        /// <summary>
        /// 检查文件夹
        /// </summary>
        protected override void CheckFolderAndCreate()
        {
            if (!Directory.Exists(FilesDirectoryPathTemp))
            {
                Directory.CreateDirectory(FilesDirectoryPathTemp);
            }

            base.CheckFolderAndCreate();
        }

        /// <summary>
        /// 从网络套接字接收文件并移动到目标的文件夹中，如果结果异常，则结束通讯
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="savename"></param>
        /// <param name="fileNameNew"></param>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <param name="filetag"></param>
        /// <param name="fileupload"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ReceiveFileFromSocketAndMoveFile(
            Socket socket,
            string savename,
            string fileNameNew,
            out string filename,
            out long size,
            out string filetag,
            out string fileupload,
            OperateResult result
            )
        {
            // 先接收文件
            if (!ReceiveFileFromSocket(
                socket,                              // 网络套接字
                savename,                            // 临时保存的完整目录
                out filename,                        // 文件名称
                out size,                            // 文件大小
                out filetag,                         // 文件描述
                out fileupload,                      // 文件的上传人
                result,                              // 操作结果对象
                null,                                // 不进行报告对象
                "接收来自客户端的文件异常"           // 需要记录的异常信息
                ))
            {
                DeleteFileByName(savename);
                return false;
            }


            // 标记移动文件，失败尝试三次
            int customer = 0;
            int times = 0;
            while (times < 3)
            {
                times++;
                if (MoveFileToNewFile(savename, fileNameNew))
                {
                    customer = 1;
                    break;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            if (customer == 0)
            {
                DeleteFileByName(savename);
            }

            // 回发消息
            if (!SendStringAndCheckReceive(
                socket,                                                                // 网络套接字
                customer,                                                              // 是否移动成功
                "成功",                                                                // 字符串数据，没意义
                result,                                                                // 结果数据对象
                null,                                                                  // 不进行报告
                "回发移动文件错误")                                                    // 发送错误时的数据
                )
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Public Method
        
        /// <summary>
        /// 用于接收上传文件时的临时文件夹，临时文件使用结束后会被删除
        /// </summary>
        public string FilesDirectoryPathTemp
        {
            get { return m_FilesDirectoryPathTemp; }
            set { m_FilesDirectoryPathTemp = PreprocessFolderName(value); }
        }

        #endregion

        #region Private Member

        private string m_FilesDirectoryPathTemp = null;

        #endregion
    }


    /// <summary>
    /// 终极文件管理服务器，实现所有的文件分类管理，读写分离，不支持直接访问文件名
    /// </summary>
    public sealed class UltimateFileServer : FileServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public UltimateFileServer()
        {
            LogHeaderText = "UltimateFileServer";
        }

        #endregion

        #region 文件列表存储器

        /// <summary>
        /// 所有文件组操作的词典锁
        /// </summary>
        internal Dictionary<string, GroupFileContainer> m_dictionary_group_marks = new Dictionary<string, GroupFileContainer>();
        /// <summary>
        /// 词典的锁
        /// </summary>
        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock();

        /// <summary>
        /// 获取当前目录的读写锁，如果没有会自动创建
        /// </summary>
        /// <param name="filePath">相对路径名</param>
        /// <returns>读写锁</returns>
        public GroupFileContainer GetGroupFromFilePath(string filePath)
        {
            GroupFileContainer GroupFile = null;
            hybirdLock.Enter();

            // lock operator
            if (m_dictionary_group_marks.ContainsKey(filePath))
            {
                GroupFile = m_dictionary_group_marks[filePath];
            }
            else
            {
                GroupFile = new GroupFileContainer(LogNet, filePath);
                m_dictionary_group_marks.Add(filePath, GroupFile);
            }

            hybirdLock.Leave();
            return GroupFile;
        }




        #endregion

        #region 接收文件数据并更新文件列表


        /// <summary>
        /// 从套接字接收文件并保存，更新文件列表
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="savename">保存的文件名</param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ReceiveFileFromSocketAndUpdateGroup(
            Socket socket,
            string savename,
            OperateResult result
            )
        {
            FileInfo info = new FileInfo(savename);
            string guidName = CreateRandomFileName();
            string fileName = info.DirectoryName + "\\" + guidName;

            if (!ReceiveFileFromSocket(
                socket,                                 // 临时保存文件路径
                fileName,                               // 最终保存文件路径
                out string FileName,                    // 文件名称，从客户端上传到服务器时，为上传人
                out long FileSize,                      // 文件的大小
                out string FileTag,                     // 文件的描述
                out string FileUpload,
                result,
                null
                ))
            {
                DeleteFileByName(fileName);
                return false;
            }


            // 更新操作
            GroupFileContainer fileManagment = GetGroupFromFilePath(info.DirectoryName);
            string oldName = fileManagment.UpdateFileMappingName(
                info.Name,
                FileSize,
                guidName,
                FileUpload,
                FileTag
                );
            
            // 删除旧的文件
            DeleteExsistingFile(info.DirectoryName, oldName);


            // 回发消息
            if (!SendStringAndCheckReceive(
                socket,                                                                // 网络套接字
                1,                                                                     // 没啥意义
                "成功",                                                                // 字符串数据，没意义
                result,                                                                // 结果数据对象
                null,                                                                  // 不进行报告
                "回发移动文件错误")                                                    // 发送错误时的数据
                )
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 根据文件的显示名称转化为真实存储的名称
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileName">文件显示名称</param>
        /// <returns></returns>
        private string TransformFactFileName(string factory, string group, string id, string fileName)
        {
            string path = ReturnAbsoluteFilePath(factory, group, id);
            GroupFileContainer fileManagment = GetGroupFromFilePath(path);
            return fileManagment.GetCurrentFileMappingName(fileName);
        }

        private void DeleteExsistingFile(string path, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string fileUltimatePath = path + "\\" + fileName;
                FileMarkId fileMarkId = GetFileMarksFromDictionaryWithFileName(fileName);

                fileMarkId.AddOperation(() =>
                {
                    if(DeleteFileByName(fileUltimatePath))
                    {
                        LogNet?.WriteInfo(LogHeaderText, "文件删除成功:" + fileUltimatePath);
                    }
                });
            }
        }

        #endregion

        #region Protect Override

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="obj"></param>
        protected override void ThreadPoolLogin(object obj)
        {
            if (obj is Socket socket)
            {

                OperateResult result = new OperateResult();

                // 获取ip地址
                string IpAddress = ((IPEndPoint)(socket.RemoteEndPoint)).Address.ToString();

                // 接收操作信息
                if (!ReceiveInformationHead(
                    socket,
                    out int customer,
                    out string fileName,
                    out string Factory,
                    out string Group,
                    out string Identify,
                    result,
                    "filename received failed.ip:" + IpAddress
                    ))
                {
                    return;
                }

                string relativeName = ReturnRelativeFileName(Factory, Group, Identify, fileName);

                if (customer == HslCommunicationCode.Hsl_Protocol_File_Download)
                {
                    // 先获取文件的真实名称
                    string guidName = TransformFactFileName(Factory, Group, Identify, fileName);
                    // 获取文件操作锁
                    FileMarkId fileMarkId = GetFileMarksFromDictionaryWithFileName(guidName);
                    fileMarkId.EnterReadOperator();
                    // 发送文件数据
                    if (!SendFileAndCheckReceive(
                        socket,
                        ReturnAbsoluteFileName(Factory, Group, Identify, guidName),
                        fileName,
                        "",
                        "",
                        result,
                        null))
                    {
                        fileMarkId.LeaveReadOperator();
                        LogNet?.WriteError(LogHeaderText,$"{StringResources.FileDownloadFailed}:{relativeName} ip:{IpAddress}");
                        return;
                    }
                    else
                    {
                        LogNet?.WriteInfo(LogHeaderText,StringResources.FileDownloadSuccess + ":" + relativeName);
                    }
                    fileMarkId.LeaveReadOperator();
                    // 关闭连接
                    socket?.Close();
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Upload)
                {
                    string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                    // 上传文件
                    CheckFolderAndCreate();
                    FileInfo info = new FileInfo(fullFileName);

                    try
                    {
                        if (!Directory.Exists(info.DirectoryName))
                        {
                            Directory.CreateDirectory(info.DirectoryName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogNet?.WriteException(LogHeaderText,"创建文件夹失败：" + fullFileName, ex);
                        socket?.Close();
                        return;
                    }

                    // 接收文件并回发消息
                    if (ReceiveFileFromSocketAndUpdateGroup(
                        socket,                    // 网络套接字
                        fullFileName,              // 完整文件名称
                        result                     // 结果数据对象
                        ))
                    {
                        socket?.Close();
                        LogNet?.WriteInfo(LogHeaderText,StringResources.FileUploadSuccess + ":" + relativeName);
                    }
                    else
                    {
                        LogNet?.WriteInfo(LogHeaderText,StringResources.FileUploadFailed + ":" + relativeName);
                    }
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Delete)
                {
                    string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);

                    FileInfo info = new FileInfo(fullFileName);
                    GroupFileContainer fileManagment = GetGroupFromFilePath(info.DirectoryName);

                    // 新增删除的任务
                    DeleteExsistingFile(info.DirectoryName, fileManagment.DeleteFile(info.Name));

                    // 回发消息
                    if (SendStringAndCheckReceive(
                        socket,                                                                // 网络套接字
                        1,                                                                     // 没啥含义
                        "成功",                                                                // 没啥含意
                        result,                                                                // 结果数据对象
                        null,                                                                  // 不进行报告
                        "回发删除结果错误")                                                    // 发送错误时的数据
                        )
                    {
                        socket?.Close();
                    }

                    LogNet?.WriteInfo(LogHeaderText,StringResources.FileDeleteSuccess + ":" + relativeName);
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Directory_Files)
                {
                    GroupFileContainer fileManagment = GetGroupFromFilePath(ReturnAbsoluteFilePath(Factory, Group, Identify));

                    if (SendStringAndCheckReceive(
                        socket,
                        HslCommunicationCode.Hsl_Protocol_File_Directory_Files,
                        fileManagment.JsonArrayContent,
                        result,
                        null,
                        "发送文件列表回客户端失败"))
                    {
                        socket?.Close();
                    }
                }
                else if (customer == HslCommunicationCode.Hsl_Protocol_File_Directories)
                {
                    List<string> folders = new List<string>();
                    foreach (var m in GetDirectories(Factory, Group, Identify))
                    {
                        DirectoryInfo directory = new DirectoryInfo(m);
                        folders.Add(directory.Name);
                    }

                    Newtonsoft.Json.Linq.JArray jArray = Newtonsoft.Json.Linq.JArray.FromObject(folders.ToArray());
                    if (SendStringAndCheckReceive(
                        socket,
                        HslCommunicationCode.Hsl_Protocol_File_Directory_Files,
                        jArray.ToString(),
                        result,
                        null,
                        "发送文件夹列表回客户端失败"))
                    {
                        socket?.Close();
                    }
                }
                else
                {
                    socket?.Close();
                }
            }
        }

        
        #endregion
        
    }

    
    #region 文件版本管理器
    

    /// <summary>
    /// 文件集容器，绑定一个文件夹的文件信息组
    /// </summary>
    public class GroupFileContainer
    {
        #region Constructor
        
        /// <summary>
        /// 实例化一个新的数据管理容器
        /// </summary>
        /// <param name="logNet">日志记录对象，可以为空</param>
        /// <param name="path"></param>
        public GroupFileContainer(ILogNet logNet, string path)
        {
            LogNet = logNet;
            GroupFileContainerLoadByPath(path);
        }

        #endregion

        #region Public Members
        
        /// <summary>
        /// 包含所有文件列表信息的json文本缓存
        /// </summary>
        public string JsonArrayContent
        {
            get { return m_JsonArrayContent; }
        }
        
        /// <summary>
        /// 获取文件的数量
        /// </summary>
        public int FileCount
        {
            get { return m_filesCount; }
        }

        #endregion

        #region Event Handle
        

        private void OnFileCountChanged()
        {
            FileCountChanged?.Invoke(m_filesCount);
        }

        /// <summary>
        /// 当文件数量发生变化的时候触发的事件
        /// </summary>
        public event Action<int> FileCountChanged;

        #endregion
        
        #region Upload Download Delete


        /// <summary>
        /// 下载文件时调用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetCurrentFileMappingName(string fileName)
        {
            string source = Guid.NewGuid().ToString("N");
            hybirdLock.Enter();
            for (int i = 0; i < m_files.Count; i++)
            {
                if (m_files[i].FileName == fileName)
                {
                    source = m_files[i].MappingName;
                    m_files[i].DownloadTimes++;
                }
            }
            hybirdLock.Leave();

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation();

            return source;
        }

        /// <summary>
        /// 上传文件时掉用
        /// </summary>
        /// <param name="fileName">文件名，带后缀，不带任何的路径</param>
        /// <param name="fileSize">文件的大小</param>
        /// <param name="mappingName">文件映射名称</param>
        /// <param name="owner">文件的拥有者</param>
        /// <param name="description">文件的额外描述</param>
        /// <returns></returns>
        public string UpdateFileMappingName(string fileName, long fileSize, string mappingName, string owner, string description)
        {
            string source = string.Empty;
            hybirdLock.Enter();

            for (int i = 0; i < m_files.Count; i++)
            {
                if (m_files[i].FileName == fileName)
                {
                    source = m_files[i].MappingName;
                    m_files[i].MappingName = mappingName;
                    m_files[i].Description = description;
                    m_files[i].FileSize = fileSize;
                    m_files[i].Owner = owner;
                    m_files[i].UploadTime = DateTime.Now;
                    break;
                }
            }

            if (string.IsNullOrEmpty(source))
            {
                // 文件不存在
                m_files.Add(new GroupFileItem()
                {
                    FileName = fileName,
                    FileSize = fileSize,
                    DownloadTimes = 0,
                    Description = description,
                    Owner = owner,
                    MappingName = mappingName,
                    UploadTime = DateTime.Now
                });
            }

            hybirdLock.Leave();

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation();

            return source;
        }

        /// <summary>
        /// 删除一个文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DeleteFile(string fileName)
        {
            string source= string.Empty;
            hybirdLock.Enter();
            for (int i = 0; i < m_files.Count; i++)
            {
                if (m_files[i].FileName == fileName)
                {
                    source = m_files[i].MappingName;
                    m_files.RemoveAt(i);
                    break;
                }
            }
            hybirdLock.Leave();

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation();
            return source;
        }
        
        #endregion

        #region Private Method
        
        /// <summary>
        /// 缓存JSON文本的方法，该机制使用乐观并发模型完成
        /// </summary>
        private void CacheJsonArrayContent()
        {
            hybirdLock.Enter();
            m_filesCount = m_files.Count;
            m_JsonArrayContent = Newtonsoft.Json.Linq.JArray.FromObject(m_files).ToString();
            
            // 保存文件
            using (StreamWriter sw = new StreamWriter(m_filePath + FileListResources, false, Encoding.UTF8))
            {
                sw.Write(m_JsonArrayContent);
            }
            
            hybirdLock.Leave();
            // 通知更新
            OnFileCountChanged();
        }

        /// <summary>
        /// 从目录进行加载数据，必须实例化的时候加载，加载失败会导致系统异常，旧的文件丢失
        /// </summary>
        /// <param name="path"></param>
        private void GroupFileContainerLoadByPath(string path)
        {
            m_filePath = path;
            
            if (!Directory.Exists(m_filePath))
            {
                Directory.CreateDirectory(m_filePath);
            }

            if (File.Exists(m_filePath + FileListResources))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(m_filePath + FileListResources, Encoding.UTF8))
                    {
                        m_files = Newtonsoft.Json.Linq.JArray.Parse(sr.ReadToEnd()).ToObject<List<GroupFileItem>>();
                    }
                }
                catch(Exception ex)
                {
                    LogNet?.WriteException("GroupFileContainer", "Load files txt failed,", ex);
                }
            }

            if (m_files == null)
            {
                m_files = new List<GroupFileItem>();
            }

            coordinatorCacheJsonArray = new HslAsyncCoordinator(CacheJsonArrayContent);

            CacheJsonArrayContent();
        }
        
        #endregion

        #region Private Members

        private const string FileListResources = "\\list.txt";            // 文件名
        private ILogNet LogNet;                                           // 日志对象
        private string m_JsonArrayContent = "[]";                         // 缓存数据
        private int m_filesCount = 0;                                     // 文件数量
        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock();     // 简单混合锁
        private HslAsyncCoordinator coordinatorCacheJsonArray;            // 乐观并发模型
        private List<GroupFileItem> m_files;                              // 文件队列
        private string m_filePath;                                        // 文件路径

        #endregion
    }




    /// <summary>
    /// 单个文件的存储
    /// </summary>
    public class GroupFileItem
    {
        #region Public Members

        /// <summary>
        /// 文件的名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件的大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件的映射名称
        /// </summary>
        public string MappingName { get; set; }
        /// <summary>
        /// 文件的下载次数
        /// </summary>
        public long DownloadTimes { get; set; }
        /// <summary>
        /// 文件的上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
        /// <summary>
        /// 文件的上传人，拥有者
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// 文件的额外描述
        /// </summary>
        public string Description { get; set; }

        
        #endregion

        #region Get Size Text

        /// <summary>
        /// 获取大小
        /// </summary>
        /// <returns></returns>
        public string GetTextFromFileSize()
        {
            return SoftBasic.GetSizeDescription(FileSize);
        }
        
        #endregion

    }







    /// <summary>
    /// 文件标记对象类
    /// </summary>
    internal class FileMarkId
    {
        /// <summary>
        /// 实例化一个文件标记对象
        /// </summary>
        public FileMarkId(ILogNet logNet, string fileName)
        {
            LogNet = logNet;
            FileName = fileName;
        }

        private ILogNet LogNet;

        private string FileName = null;

        private Queue<Action> queues = new Queue<Action>();

        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock();


        /// <summary>
        /// 新增一个文件的操作，仅仅是删除文件
        /// </summary>
        /// <param name="action"></param>
        public void AddOperation(Action action)
        {
            hybirdLock.Enter();
            
            if (readStatus == 0)
            {
                // 没有读取状态，立马执行
                action?.Invoke();
            }
            else
            {
                // 添加标记
                queues.Enqueue(action);
            }
            hybirdLock.Leave();
        }

        
        private int readStatus = 0;

        /// <summary>
        /// 指示该对象是否能被清除
        /// </summary>
        /// <returns></returns>
        public bool CanClear()
        {
            bool result = false;
            hybirdLock.Enter();
            result = readStatus == 0 && queues.Count == 0;
            hybirdLock.Leave();
            return result;
        }

        /// <summary>
        /// 进入文件的读取状态
        /// </summary>
        public void EnterReadOperator()
        {
            hybirdLock.Enter();
            readStatus++;
            hybirdLock.Leave();
        }

        /// <summary>
        /// 离开本次的文件读取状态
        /// </summary>
        public void LeaveReadOperator()
        {
            // 检查文件标记状态
            hybirdLock.Enter();
            readStatus--;
            if (readStatus == 0)
            {
                while (queues.Count > 0)
                {
                    try
                    {
                        queues.Dequeue()?.Invoke();
                    }
                    catch(Exception ex)
                    {
                        LogNet?.WriteException("FileMarkId", "File Action Failed:", ex);
                    }
                }
            }
            hybirdLock.Leave();
        }
    }


    #endregion
}
