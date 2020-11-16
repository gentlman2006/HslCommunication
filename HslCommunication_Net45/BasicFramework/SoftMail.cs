using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace HslCommunication.BasicFramework
{

    /****************************************************************************
     * 
     *    创建日期：2017-02-27 10:29:31
     *    功能：用来发送简单的邮件
     *    说明：使用代理的方式来实现发送邮件
     *    备注：静态对象直接提供了一个可供发送的163邮件和QQ邮件
     * 
     ***************************************************************************/







    /// <summary>
    /// 软件的邮箱类，用于发送邮箱数据
    /// </summary>
    /// <remarks>
    /// 如果您想实现自己的邮件发送中心，就可以去对应的邮件服务器注册，如果是想快速实现邮件的发送，本系统提供了2个静态的已经注册好了的邮箱发送器。
    /// </remarks>
    /// <example>
    /// 以下的代码演示了通过静态的发送对象来发送邮件，包含了发送普通的邮件，和发送html邮件。
    /// <code lang="cs" source="TestProject\HslCommunicationDemo\FormMail.cs" region="SoftMail" title="SoftMail示例" />
    /// </example>
    public class SoftMail
    {
        #region Static Member

        /// <summary>
        /// 系统连续发送失败的次数，为了不影响系统，连续三次失败就禁止发送
        /// </summary>
        private static long SoftMailSendFailedCount { get; set; } = 0;

        #endregion

        #region Static Mail


        /// <summary>
        /// 系统提供一个默认的163邮箱发送账号，只要更改接收地址即可发送服务，可能会被拦截
        /// </summary>
        public static SoftMail MailSystem163 = new SoftMail(
            mail =>
            {
                mail.Host = "smtp.163.com";//使用163的SMTP服务器发送邮件
                mail.UseDefaultCredentials = true;
                mail.EnableSsl = true;
                mail.Port = 25;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.Credentials = new System.Net.NetworkCredential("softmailsendcenter", "zxcvbnm6789");//密码zxcvbnm1234
            },
            "softmailsendcenter@163.com",
            "hsl200909@163.com"
            );

        /// <summary>
        /// 系统提供一个默认的QQ邮箱发送账号，只要更改接收地址即可发送服务，发送成功概率比较高
        /// </summary>
        public static SoftMail MailSystemQQ = new SoftMail(
            mail =>
            {
                mail.Host = "smtp.qq.com";//使用QQ的SMTP服务器发送邮件
                mail.UseDefaultCredentials = true;
                mail.Port = 587;
                mail.EnableSsl = true;
                mail.DeliveryMethod = SmtpDeliveryMethod.Network;
                mail.Credentials = new System.Net.NetworkCredential("974856779", "tvnlczxdumutbbic");//密码tvnlczxdumutbbic
            },
            "974856779@qq.com",
            "hsl200909@163.com"
            );

        #endregion

        #region Constructor


        /// <summary>
        /// 实例化一个邮箱发送类，需要指定初始化信息
        /// </summary>
        /// <param name="mailIni">初始化的方法</param>
        /// <param name="addr_From">发送地址，应该和账户匹配</param>
        /// <param name="addr_to">邮件接收地址</param>
        /// <remarks>
        /// 初始化的方法比较复杂，需要参照示例代码。
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45\BasicFramework\SoftMail.cs" region="Static Mail" title="SoftMail示例" />
        /// </example>
        public SoftMail(Action<SmtpClient> mailIni, string addr_From = "", string addr_to = "")
        {
            smtpClient = new SmtpClient();
            mailIni(smtpClient);
            MailFromAddress = addr_From;
            MailSendAddress = addr_to;
        }


        #endregion

        #region Private Member
        
        /// <summary>
        /// 系统的邮件发送客户端
        /// </summary>
        private SmtpClient smtpClient { get; set; }
        /// <summary>
        /// 发送邮件的地址
        /// </summary>
        private string MailFromAddress { get; set; } = "";

        #endregion

        #region Public Member


        /// <summary>
        /// 邮件发送的地址
        /// </summary>
        public string MailSendAddress { get; set; } = "";


        #endregion

        #region Private Method


        private string GetExceptionMail(Exception ex)
        {
            return $"{StringResources.Language.Time}{DateTime.Now.ToString()}" +
                $"{Environment.NewLine}{StringResources.Language.SoftWare}{ex.Source}" +
                $"{Environment.NewLine}{StringResources.Language.ExceptionMessage}{ex.Message}" +
                $"{Environment.NewLine}{StringResources.Language.ExceptionType}{ex.GetType().ToString()}" +
                $"{Environment.NewLine}{StringResources.Language.ExceptionStackTrace}{ex.StackTrace}" +
                $"{Environment.NewLine}{StringResources.Language.ExceptopnTargetSite}{ex.TargetSite.Name}";
        }

        #endregion

        #region SendMail Support


        /// <summary>
        /// 发生BUG至邮件地址，需要提前指定发送地址，否则失败
        /// </summary>
        /// <param name="ex">异常的BUG，同样试用兼容类型</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(Exception ex)
        {
            return SendMail(ex, "");
        }

        /// <summary>
        /// 发送邮件至地址，需要提前指定发送地址，否则失败
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string subject, string body)
        {
            return SendMail(MailSendAddress, subject, body);
        }

        /// <summary>
        /// 发送邮件至地址，需要提前指定发送地址，否则失败
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isHtml">是否是html格式化文本</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string subject, string body, bool isHtml)
        {
            return SendMail(MailSendAddress, subject, body, isHtml);
        }


        /// <summary>
        /// 发生BUG至邮件地址，需要提前指定发送地址，否则失败
        /// </summary>
        /// <param name="ex">异常的BUG，同样试用兼容类型</param>
        /// <param name="addtion">额外信息</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(Exception ex, string addtion)
        {
            if (string.IsNullOrEmpty(MailSendAddress)) return false;
            return SendMail(MailSendAddress, StringResources.Language.BugSubmit, string.IsNullOrEmpty(addtion) ? GetExceptionMail(ex) :
                $"User：{addtion}{Environment.NewLine}" + GetExceptionMail(ex));
        }

        /// <summary>
        /// 发送邮件的方法，需要指定接收地址，主题及内容
        /// </summary>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string addr_to, string subject, string body)
        {
            return SendMail(addr_to, subject, body, false);
        }

        /// <summary>
        /// 发送邮件的方法，默认发送别名，优先级，是否HTML
        /// </summary>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isHtml">是否是html格式的内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string addr_to, string subject, string body, bool isHtml)
        {
            return SendMail(MailFromAddress, StringResources.Language.MailServerCenter, new string[] { addr_to }, subject, body, MailPriority.Normal, isHtml);
        }

        /// <summary>
        /// 发送邮件的方法，需要提供完整的参数信息
        /// </summary>
        /// <param name="addr_from">发送地址</param>
        /// <param name="name">发送别名</param>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="isHtml">邮件内容是否是HTML语言</param>
        /// <returns>发生是否成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string addr_from, string name, string[] addr_to, string subject, string body, MailPriority priority, bool isHtml)
        {
            if (SoftMailSendFailedCount > 10) { SoftMailSendFailedCount++; return true; }

            using (MailMessage Message = new MailMessage())
            {
                try
                {
                    Message.From = new MailAddress(addr_from, name, Encoding.UTF8);
                    foreach (var m in addr_to)
                    {
                        Message.To.Add(m);
                    }
                    Message.Subject = subject;
                    Message.Body = body;
                    Message.Body += Environment.NewLine + Environment.NewLine + Environment.NewLine + StringResources.Language.MailSendTail;
                    Message.SubjectEncoding = Encoding.UTF8;
                    Message.BodyEncoding = Encoding.UTF8;
                    Message.Priority = priority;
                    Message.IsBodyHtml = isHtml;
                    smtpClient.Send(Message);

                    //清空数据
                    SoftMailSendFailedCount = 0;
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine( SoftBasic.GetExceptionMessage( ex ) );
                    SoftMailSendFailedCount++;
                    return false;
                }
            }
        }


        #endregion

    }
}
