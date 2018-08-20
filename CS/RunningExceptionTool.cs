using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SciTools
{
    //// 示例：Program.cs
    //static class Program
    //{
    //    /// <summary>
    //    /// 应用程序的主入口点。
    //    /// </summary>
    //    [STAThread]
    //    static void Main(string[] args = null)
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);

    //        SciTools.RunningExceptionTool.Run(call);                     // 捕获运行异常
    //        //SciTools.RunningExceptionTool.Run(call, SendErrors);       // 捕获运行异常，并发送异常信息
    //        //SciTools.RunningExceptionTool.Run(call, SendErrors, args); // 捕获运行异常，并发送异常信息, Main方法有参数
    //    }

    //    /// <summary>
    //    /// 应用程序，入口逻辑
    //    /// </summary>
    //    public static void call(string[] args = null)
    //    {
    //        Application.Run(new Form1());
    //    }

    //    /// <summary>
    //    // 自定义异常信息处理逻辑
    //    /// </summary>
    //    public static void SendErrors(string errorMsg)
    //    {
    //        // 自定义异常信息处理逻辑
    //    }
    //}

    /// <summary>
    /// 捕获运行时异常信息
    /// </summary>
    public class RunningExceptionTool
    {
        /// <summary>
        /// 定义委托接口处理函数，调用此类中的Main函数为应用添加异常信息捕获
        /// </summary>
        public delegate void MainFunction(string[] args = null);

        /// <summary>
        /// 异常信息回调处理逻辑
        /// </summary>
        public delegate void ExceptionMsg(string msg);
        private static ExceptionMsg Excall = null;


        public static void Run(MainFunction main, ExceptionMsg ExCall = null, string[] args = null)
        {
            try
            {
                if (Excall != null) RunningExceptionTool.Excall = ExCall;

                // 捕获
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(CatchThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CatchUnhandledException);

                if (main != null)
                {
                    if (args == null) main();
                    else main(args);
                }
            }
            catch (Exception ex)
            {
                string str = GetExceptionMsg(ex, string.Empty);
                MessageBox.Show(str, "运行异常", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //bool ok = (MessageBox.Show(str, "运行异常，提交bug信息？", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK);
                //if (ok) sendBugToAuthor(str);

                if (Excall != null) Excall(str);
            }
        }

        /// <summary>
        /// 捕获线程异常
        /// </summary>
        static void CatchThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            MessageBox.Show(str, "运行异常", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (Excall != null) Excall(str);
        }

        /// <summary>
        /// 捕获未处理异常
        /// </summary>
        static void CatchUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(str, "运行异常", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (Excall != null) Excall(str);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常信息****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
                sb.AppendLine("【异常方法】：" + ex.TargetSite);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");

            return sb.ToString();
        }
    }
}
