using HslCommunication.Core;
using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HslCommunication.Enthernet
{
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
        /// <param name="path">文件的路径</param>
        public GroupFileContainer( ILogNet logNet, string path )
        {
            LogNet = logNet;
            GroupFileContainerLoadByPath( path );
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


        private void OnFileCountChanged( )
        {
            FileCountChanged?.Invoke( m_filesCount );
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
        /// <param name="fileName">文件的实际名称</param>
        /// <returns>文件名映射过去的实际的文件名字</returns>
        public string GetCurrentFileMappingName( string fileName )
        {
            string source = Guid.NewGuid( ).ToString( "N" );
            hybirdLock.Enter( );
            for (int i = 0; i < m_files.Count; i++)
            {
                if (m_files[i].FileName == fileName)
                {
                    source = m_files[i].MappingName;
                    m_files[i].DownloadTimes++;
                }
            }
            hybirdLock.Leave( );

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation( );

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
        /// <returns>映射的文件名称</returns>
        public string UpdateFileMappingName( string fileName, long fileSize, string mappingName, string owner, string description )
        {
            string source = string.Empty;
            hybirdLock.Enter( );

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

            if (string.IsNullOrEmpty( source ))
            {
                // 文件不存在
                m_files.Add( new GroupFileItem( )
                {
                    FileName = fileName,
                    FileSize = fileSize,
                    DownloadTimes = 0,
                    Description = description,
                    Owner = owner,
                    MappingName = mappingName,
                    UploadTime = DateTime.Now
                } );
            }

            hybirdLock.Leave( );

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation( );

            return source;
        }

        /// <summary>
        /// 删除一个文件信息
        /// </summary>
        /// <param name="fileName">实际的文件名称</param>
        /// <returns>映射之后的文件名</returns>
        public string DeleteFile( string fileName )
        {
            string source = string.Empty;
            hybirdLock.Enter( );
            for (int i = 0; i < m_files.Count; i++)
            {
                if (m_files[i].FileName == fileName)
                {
                    source = m_files[i].MappingName;
                    m_files.RemoveAt( i );
                    break;
                }
            }
            hybirdLock.Leave( );

            // 更新缓存
            coordinatorCacheJsonArray.StartOperaterInfomation( );
            return source;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 缓存JSON文本的方法，该机制使用乐观并发模型完成
        /// </summary>
        private void CacheJsonArrayContent( )
        {
            hybirdLock.Enter( );
            m_filesCount = m_files.Count;
            m_JsonArrayContent = Newtonsoft.Json.Linq.JArray.FromObject( m_files ).ToString( );

            // 保存文件
            using (StreamWriter sw = new StreamWriter( m_filePath + FileListResources, false, Encoding.UTF8 ))
            {
                sw.Write( m_JsonArrayContent );
            }

            hybirdLock.Leave( );
            // 通知更新
            OnFileCountChanged( );
        }

        /// <summary>
        /// 从目录进行加载数据，必须实例化的时候加载，加载失败会导致系统异常，旧的文件丢失
        /// </summary>
        /// <param name="path"></param>
        private void GroupFileContainerLoadByPath( string path )
        {
            m_filePath = path;

            if (!Directory.Exists( m_filePath ))
            {
                Directory.CreateDirectory( m_filePath );
            }

            if (File.Exists( m_filePath + FileListResources ))
            {
                try
                {
                    using (StreamReader sr = new StreamReader( m_filePath + FileListResources, Encoding.UTF8 ))
                    {
                        m_files = Newtonsoft.Json.Linq.JArray.Parse( sr.ReadToEnd( ) ).ToObject<List<GroupFileItem>>( );
                    }
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( "GroupFileContainer", "Load files txt failed,", ex );
                }
            }

            if (m_files == null)
            {
                m_files = new List<GroupFileItem>( );
            }

            coordinatorCacheJsonArray = new HslAsyncCoordinator( CacheJsonArrayContent );

            CacheJsonArrayContent( );
        }

        #endregion

        #region Private Members

        private const string FileListResources = "\\list.txt";            // 文件名
        private ILogNet LogNet;                                           // 日志对象
        private string m_JsonArrayContent = "[]";                         // 缓存数据
        private int m_filesCount = 0;                                     // 文件数量
        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock( );     // 简单混合锁
        private HslAsyncCoordinator coordinatorCacheJsonArray;            // 乐观并发模型
        private List<GroupFileItem> m_files;                              // 文件队列
        private string m_filePath;                                        // 文件路径

        #endregion
    }
}
